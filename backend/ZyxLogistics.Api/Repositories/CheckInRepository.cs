using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Services;

namespace ZyxLogistics.Api.Repositories
{
    public class CheckInRepository : ICheckInRepository
    {
        private readonly DbConnectionFactory _connectionFactory;
        private readonly ISmsService _smsService;
        private readonly IConfiguration _configuration;

        public CheckInRepository(
            DbConnectionFactory connectionFactory,
            ISmsService smsService,
            IConfiguration configuration)
        {
            _connectionFactory = connectionFactory;
            _smsService = smsService;
            _configuration = configuration;
        }

        public async Task<CheckInCodigoResponse> SolicitarCodigoAsync(SolicitarCheckInCodigoRequest request)
        {
            var codigo = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_CheckIn_SolicitarCodigo");
            command.Parameters.Add("@Cnh", SqlDbType.VarChar, 30).Value = request.Cnh.Trim();
            command.Parameters.Add("@Codigo", SqlDbType.VarChar, 6).Value = codigo;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new InvalidOperationException("Codigo de check-in nao foi gerado.");
            }

            var response = new CheckInCodigoResponse
            {
                AgendamentoId = reader.GetInt32("AgendamentoId"),
                MotoristaNome = reader.GetString("MotoristaNome"),
                TelefoneMascarado = reader.GetString("TelefoneMascarado"),
                ExpiraEm = reader.GetDateTime("ExpiraEm"),
                CodigoDesenvolvimento = IsSimulatedSms() ? reader.GetString("CodigoDesenvolvimento") : null,
                Telefone = reader.GetString("Telefone"),
                Mensagem = reader.GetString("Mensagem")
            };

            await _smsService.EnviarAsync(response.Telefone, response.Mensagem);

            return response;
        }

        public async Task<bool> ConfirmarAsync(ConfirmarCheckInRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_CheckIn_Confirmar");
            command.Parameters.Add("@Cnh", SqlDbType.VarChar, 30).Value = request.Cnh.Trim();
            command.Parameters.Add("@Codigo", SqlDbType.VarChar, 6).Value = request.Codigo.Trim();

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        private static SqlCommand CreateProcedureCommand(SqlConnection connection, string procedureName)
        {
            return new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private bool IsSimulatedSms()
        {
            var provider = _configuration["Sms:Provider"];
            return string.Equals(provider, "Simulated", StringComparison.OrdinalIgnoreCase);
        }
    }
}
