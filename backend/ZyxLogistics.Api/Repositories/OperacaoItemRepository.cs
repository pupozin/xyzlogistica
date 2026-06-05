using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class OperacaoItemRepository : IOperacaoItemRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public OperacaoItemRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<OperacaoItem>> ListarPorAgendamentoAsync(int agendamentoId)
        {
            var itens = new List<OperacaoItem>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_OperacaoItem_ListarPorAgendamento");
            command.Parameters.Add("@AgendamentoId", SqlDbType.Int).Value = agendamentoId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                itens.Add(MapOperacaoItem(reader));
            }

            return itens;
        }

        public async Task<int> InserirAsync(int agendamentoId, OperacaoItemCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_OperacaoItem_Inserir");
            command.Parameters.Add("@AgendamentoId", SqlDbType.Int).Value = agendamentoId;
            command.Parameters.Add("@ProdutoId", SqlDbType.Int).Value = request.ProdutoId;
            command.Parameters.Add("@Quantidade", SqlDbType.Decimal).Value = request.Quantidade;
            command.Parameters["@Quantidade"].Precision = 18;
            command.Parameters["@Quantidade"].Scale = 3;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> AtualizarAsync(int agendamentoId, int id, OperacaoItemUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_OperacaoItem_Atualizar");
            command.Parameters.Add("@AgendamentoId", SqlDbType.Int).Value = agendamentoId;
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@Quantidade", SqlDbType.Decimal).Value = request.Quantidade;
            command.Parameters["@Quantidade"].Precision = 18;
            command.Parameters["@Quantidade"].Scale = 3;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExcluirAsync(int agendamentoId, int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_OperacaoItem_Excluir");
            command.Parameters.Add("@AgendamentoId", SqlDbType.Int).Value = agendamentoId;
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

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

        private static OperacaoItem MapOperacaoItem(SqlDataReader reader)
        {
            return new OperacaoItem
            {
                Id = reader.GetInt32("Id"),
                AgendamentoId = reader.GetInt32("AgendamentoId"),
                ProdutoId = reader.GetInt32("ProdutoId"),
                ProdutoDescricao = reader.GetString("ProdutoDescricao"),
                Quantidade = reader.GetDecimal("Quantidade"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
