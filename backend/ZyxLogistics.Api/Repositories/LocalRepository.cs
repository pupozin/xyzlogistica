using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class LocalRepository : ILocalRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public LocalRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Local>> ListarAsync(LocalFilterRequest filter)
        {
            var locais = new List<Local>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Local_Listar");
            AddNullableStringParameter(command, "@Descricao", SqlDbType.NVarChar, 100, filter.Descricao);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                locais.Add(MapLocal(reader));
            }

            return locais;
        }

        public async Task<Local?> ObterPorIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Local_ObterPorId");
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapLocal(reader);
        }

        public async Task<int> InserirAsync(LocalCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Local_Inserir");
            AddLocalParameters(command, request.Descricao);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> AtualizarAsync(int id, LocalUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Local_Atualizar");
            command.Parameters.AddWithValue("@Id", id);
            AddLocalParameters(command, request.Descricao);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Local_Excluir");
            command.Parameters.AddWithValue("@Id", id);

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

        private static void AddLocalParameters(SqlCommand command, string descricao)
        {
            command.Parameters.Add("@Descricao", SqlDbType.NVarChar, 100).Value = descricao.Trim();
        }

        private static void AddNullableStringParameter(SqlCommand command, string name, SqlDbType type, int size, string? value)
        {
            var trimmedValue = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            command.Parameters.Add(name, type, size).Value = trimmedValue is null ? DBNull.Value : trimmedValue;
        }

        private static Local MapLocal(SqlDataReader reader)
        {
            return new Local
            {
                Id = reader.GetInt32("Id"),
                Descricao = reader.GetString("Descricao"),
                Ativo = reader.GetBoolean("Ativo"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
