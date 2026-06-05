using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class InventarioRepository : IInventarioRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public InventarioRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<InventarioItem>> ListarAsync(InventarioFilterRequest filter)
        {
            var itens = new List<InventarioItem>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand("dbo.sp_Inventario_Listar", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            var produto = string.IsNullOrWhiteSpace(filter.Produto) ? null : filter.Produto.Trim();
            command.Parameters.Add("@Produto", SqlDbType.NVarChar, 150).Value = produto is null ? DBNull.Value : produto;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                itens.Add(new InventarioItem
                {
                    ProdutoId = reader.GetInt32("ProdutoId"),
                    ProdutoDescricao = reader.GetString("ProdutoDescricao"),
                    QuantidadeAtual = reader.GetDecimal("QuantidadeAtual"),
                    DataUltimaAtualizacao = reader.GetDateTime("DataUltimaAtualizacao")
                });
            }

            return itens;
        }
    }
}
