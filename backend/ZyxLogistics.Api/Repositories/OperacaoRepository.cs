using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class OperacaoRepository : IOperacaoRepository
    {
        private const int OperacaoInboundId = 1;
        private const int OperacaoOutboundId = 2;
        private const int StatusCheckInRealizadoId = 2;
        private const int StatusEmDocaId = 3;
        private const int StatusFinalizadoId = 4;

        private readonly DbConnectionFactory _connectionFactory;

        public OperacaoRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<OperacaoAba>> ListarAbasAsync(string operacao, string? busca)
        {
            var operacaoId = ObterOperacaoId(operacao);
            var agendamentos = new List<Agendamento>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("dbo.sp_Operacao_ListarAbas", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@OperacaoId", SqlDbType.Int).Value = operacaoId;
            command.Parameters.Add("@Busca", SqlDbType.NVarChar, 150).Value =
                string.IsNullOrWhiteSpace(busca) ? DBNull.Value : busca.Trim();

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                agendamentos.Add(MapAgendamento(reader));
            }

            return new[]
            {
                CriarAba(StatusCheckInRealizadoId, "CheckInRealizado", "Aguardando Doca", agendamentos),
                CriarAba(StatusEmDocaId, "EmDoca", "Em Doca", agendamentos),
                CriarAba(StatusFinalizadoId, "Finalizado", "Finalizados", agendamentos)
            };
        }

        private static int ObterOperacaoId(string operacao)
        {
            return operacao.ToLowerInvariant() switch
            {
                "inbound" => OperacaoInboundId,
                "outbound" => OperacaoOutboundId,
                _ => throw new ArgumentException("Operacao invalida. Use inbound ou outbound.", nameof(operacao))
            };
        }

        private static OperacaoAba CriarAba(
            int statusId,
            string statusDescricao,
            string titulo,
            IReadOnlyList<Agendamento> agendamentos)
        {
            var itens = agendamentos
                .Where(agendamento => agendamento.StatusId == statusId)
                .ToList();

            return new OperacaoAba
            {
                StatusId = statusId,
                StatusDescricao = statusDescricao,
                Titulo = titulo,
                Quantidade = itens.Count,
                Agendamentos = itens
            };
        }

        private static Agendamento MapAgendamento(SqlDataReader reader)
        {
            return new Agendamento
            {
                Id = reader.GetInt32("Id"),
                OperacaoId = reader.GetInt32("OperacaoId"),
                OperacaoDescricao = reader.GetString("OperacaoDescricao"),
                StatusId = reader.GetInt32("StatusId"),
                StatusDescricao = reader.GetString("StatusDescricao"),
                VeiculoId = reader.GetInt32("VeiculoId"),
                VeiculoPlaca = reader.GetString("VeiculoPlaca"),
                TipoVeiculo = reader.GetString("TipoVeiculo"),
                TransportadoraId = reader.GetInt32("TransportadoraId"),
                TransportadoraNome = reader.GetString("TransportadoraNome"),
                LocalId = reader.IsDBNull("LocalId") ? null : reader.GetInt32("LocalId"),
                LocalDescricao = reader.IsDBNull("LocalDescricao") ? null : reader.GetString("LocalDescricao"),
                MotoristaId = reader.GetInt32("MotoristaId"),
                MotoristaNome = reader.GetString("MotoristaNome"),
                MotoristaCnh = reader.GetString("MotoristaCnh"),
                DataHoraAgendada = reader.GetDateTime("DataHoraAgendada"),
                DataHoraChegada = reader.IsDBNull("DataHoraChegada") ? null : reader.GetDateTime("DataHoraChegada"),
                DataHoraDoca = reader.IsDBNull("DataHoraDoca") ? null : reader.GetDateTime("DataHoraDoca"),
                DataHoraFinalizado = reader.IsDBNull("DataHoraFinalizado") ? null : reader.GetDateTime("DataHoraFinalizado"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
