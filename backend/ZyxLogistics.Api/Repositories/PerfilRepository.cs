using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PerfilRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Perfil>> ListarPerfisAsync()
        {
            var perfis = new List<Perfil>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Perfil_Listar");

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                perfis.Add(new Perfil
                {
                    Id = reader.GetInt32("Id"),
                    Descricao = reader.GetString("Descricao"),
                    Ativo = reader.GetBoolean("Ativo")
                });
            }

            return perfis;
        }

        public async Task<Perfil?> ObterPerfilPorIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Perfil_ObterPorId");
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapPerfil(reader);
        }

        public async Task<Perfil> InserirPerfilAsync(PerfilCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Perfil_Inserir");
            command.Parameters.Add("@Descricao", SqlDbType.NVarChar, 100).Value = request.Descricao.Trim();
            AddPermissoesParameter(command, request.PermissaoIds);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new InvalidOperationException("A procedure nao retornou o perfil criado.");
            }

            return MapPerfil(reader);
        }

        public async Task<Perfil?> AtualizarPerfilAsync(int id, PerfilUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Perfil_Atualizar");
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@Descricao", SqlDbType.NVarChar, 100).Value = request.Descricao.Trim();
            AddPermissoesParameter(command, request.PermissaoIds);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapPerfil(reader);
        }

        public async Task<bool> ExcluirPerfilAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Perfil_Excluir");
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<IReadOnlyList<Permissao>> ListarPermissoesAsync()
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Permissao_Listar");

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            return await ReadPermissoesAsync(reader);
        }

        public async Task<IReadOnlyList<Permissao>> ListarPermissoesPorPerfilAsync(int perfilId)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Permissao_ListarPorPerfil");
            command.Parameters.Add("@PerfilId", SqlDbType.Int).Value = perfilId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            return await ReadPermissoesAsync(reader);
        }

        public async Task<IReadOnlyList<Permissao>> AtualizarPermissoesAsync(int perfilId, PerfilPermissoesUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Perfil_AtualizarPermissoes");
            command.Parameters.Add("@PerfilId", SqlDbType.Int).Value = perfilId;
            AddPermissoesParameter(command, request.PermissaoIds);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            return await ReadPermissoesAsync(reader);
        }

        private static SqlCommand CreateProcedureCommand(SqlConnection connection, string procedureName)
        {
            return new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        private static void AddPermissoesParameter(SqlCommand command, IEnumerable<int> permissaoIds)
        {
            var permissoesParameter = command.Parameters.Add("@Permissoes", SqlDbType.Structured);
            permissoesParameter.TypeName = "dbo.IntIdList";
            permissoesParameter.Value = CreatePermissoesTable(permissaoIds);
        }

        private static DataTable CreatePermissoesTable(IEnumerable<int> permissaoIds)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));

            foreach (var permissaoId in permissaoIds.Where(id => id > 0).Distinct())
            {
                table.Rows.Add(permissaoId);
            }

            return table;
        }

        private static async Task<IReadOnlyList<Permissao>> ReadPermissoesAsync(SqlDataReader reader)
        {
            var permissoes = new List<Permissao>();

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

        private static Perfil MapPerfil(SqlDataReader reader)
        {
            return new Perfil
            {
                Id = reader.GetInt32("Id"),
                Descricao = reader.GetString("Descricao"),
                Ativo = reader.GetBoolean("Ativo")
            };
        }
    }
}
