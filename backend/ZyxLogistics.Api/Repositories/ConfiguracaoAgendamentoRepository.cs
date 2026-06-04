using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class ConfiguracaoAgendamentoRepository : IConfiguracaoAgendamentoRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ConfiguracaoAgendamentoRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ConfiguracaoAgendamento?> ObterAtivaAsync()
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_ConfiguracaoAgendamento_ObterAtiva");

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapConfiguracao(reader);
        }

        public async Task<ConfiguracaoAgendamento> AtualizarAsync(ConfiguracaoAgendamentoUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_ConfiguracaoAgendamento_Atualizar");
            command.Parameters.Add("@IntervaloMinutos", SqlDbType.Int).Value = request.IntervaloMinutos;
            command.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = request.HoraInicio;
            command.Parameters.Add("@HoraFim", SqlDbType.Time).Value = request.HoraFim;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new InvalidOperationException("A procedure nao retornou a configuracao atualizada.");
            }

            return MapConfiguracao(reader);
        }

        private static SqlCommand CreateProcedureCommand(SqlConnection connection, string procedureName)
        {
            return new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static ConfiguracaoAgendamento MapConfiguracao(SqlDataReader reader)
        {
            return new ConfiguracaoAgendamento
            {
                Id = reader.GetInt32("Id"),
                IntervaloMinutos = reader.GetInt32("IntervaloMinutos"),
                HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("HoraInicio")),
                HoraFim = reader.GetTimeSpan(reader.GetOrdinal("HoraFim")),
                Ativo = reader.GetBoolean("Ativo"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
