using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AuthRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<AuthUsuario?> ObterUsuarioPorEmailAsync(string email)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_Login");
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email.Trim().ToLowerInvariant();

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new AuthUsuario
            {
                Id = reader.GetInt32("Id"),
                Nome = reader.GetString("Nome"),
                Email = reader.GetString("Email"),
                Senha = reader.IsDBNull("Senha") ? null : reader.GetString("Senha"),
                PerfilId = reader.GetInt32("PerfilId"),
                PerfilDescricao = reader.GetString("PerfilDescricao"),
                PrimeiroAcesso = reader.GetBoolean("PrimeiroAcesso"),
                Ativo = reader.GetBoolean("Ativo")
            };
        }

        public async Task<bool> DefinirSenhaPrimeiroAcessoAsync(string email, string senhaHash)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_DefinirSenhaPrimeiroAcesso");
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email.Trim().ToLowerInvariant();
            command.Parameters.Add("@Senha", SqlDbType.NVarChar, 255).Value = senhaHash;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<IReadOnlyList<Permissao>> ListarPermissoesAsync(int usuarioId)
        {
            var permissoes = new List<Permissao>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Usuario_ListarPermissoes");
            command.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                permissoes.Add(new Permissao
                {
                    Id = reader.GetInt32("Id"),
                    Codigo = reader.GetString("Codigo"),
                    Descricao = reader.GetString("Descricao"),
                    Ativo = reader.GetBoolean("Ativo")
                });
            }

            return permissoes;
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
