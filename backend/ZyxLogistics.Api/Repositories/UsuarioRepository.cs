using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UsuarioRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Usuario>> ListarAsync(UsuarioFilterRequest filter)
        {
            var usuarios = new List<Usuario>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_Listar");
            AddNullableStringParameter(command, "@Nome", SqlDbType.NVarChar, 150, filter.Nome);
            AddNullableStringParameter(command, "@Email", SqlDbType.NVarChar, 150, filter.Email);
            command.Parameters.Add("@PerfilId", SqlDbType.Int).Value = filter.PerfilId.HasValue ? filter.PerfilId.Value : DBNull.Value;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuarios.Add(MapUsuario(reader));
            }

            return usuarios;
        }

        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_ObterPorId");
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapUsuario(reader);
        }

        public async Task<int> InserirAsync(UsuarioCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_Inserir");
            AddUsuarioParameters(command, request.Nome, request.Email, request.PerfilId);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> AtualizarAsync(int id, UsuarioUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_Atualizar");
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            AddUsuarioParameters(command, request.Nome, request.Email, request.PerfilId);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_Excluir");
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

        private static void AddUsuarioParameters(SqlCommand command, string nome, string email, int perfilId)
        {
            command.Parameters.Add("@Nome", SqlDbType.NVarChar, 150).Value = nome.Trim();
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email.Trim().ToLowerInvariant();
            command.Parameters.Add("@PerfilId", SqlDbType.Int).Value = perfilId;
        }

        private static void AddNullableStringParameter(SqlCommand command, string name, SqlDbType type, int size, string? value)
        {
            var trimmedValue = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            command.Parameters.Add(name, type, size).Value = trimmedValue is null ? DBNull.Value : trimmedValue;
        }

        private static Usuario MapUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32("Id"),
                Nome = reader.GetString("Nome"),
                Email = reader.GetString("Email"),
                PerfilId = reader.GetInt32("PerfilId"),
                PerfilDescricao = reader.GetString("PerfilDescricao"),
                PrimeiroAcesso = reader.GetBoolean("PrimeiroAcesso"),
                Ativo = reader.GetBoolean("Ativo"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
