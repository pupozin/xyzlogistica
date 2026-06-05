using System.Data;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;

namespace ZyxLogistics.Api.Repositories
{
    public class CheckInRepository : ICheckInRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public CheckInRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
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

            return new CheckInCodigoResponse
            {
                AgendamentoId = reader.GetInt32("AgendamentoId"),
                MotoristaNome = reader.GetString("MotoristaNome"),
                TelefoneMascarado = reader.GetString("TelefoneMascarado"),
                ExpiraEm = reader.GetDateTime("ExpiraEm"),
                CodigoDesenvolvimento = reader.GetString("CodigoDesenvolvimento")
            };
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
    }
}
