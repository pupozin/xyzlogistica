using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class MotoristaRepository : IMotoristaRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public MotoristaRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Motorista>> ListarAsync(MotoristaFilterRequest filter)
        {
            var motoristas = new List<Motorista>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Motorista_Listar");
            AddNullableStringParameter(command, "@Nome", SqlDbType.NVarChar, 150, filter.Nome);
            AddNullableStringParameter(command, "@Cnh", SqlDbType.VarChar, 30, filter.Cnh);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                motoristas.Add(MapMotorista(reader));
            }

            return motoristas;
        }

        public async Task<Motorista?> ObterPorIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Motorista_ObterPorId");
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapMotorista(reader);
        }

        public async Task<int> InserirAsync(MotoristaCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Motorista_Inserir");
            AddMotoristaParameters(command, request.Nome, request.Cnh);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> AtualizarAsync(int id, MotoristaUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Motorista_Atualizar");
            command.Parameters.AddWithValue("@Id", id);
            AddMotoristaParameters(command, request.Nome, request.Cnh);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Motorista_Excluir");
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

        private static void AddMotoristaParameters(SqlCommand command, string nome, string cnh)
        {
            command.Parameters.Add("@Nome", SqlDbType.NVarChar, 150).Value = nome.Trim();
            command.Parameters.Add("@Cnh", SqlDbType.VarChar, 30).Value = cnh.Trim();
        }

        private static void AddNullableStringParameter(SqlCommand command, string name, SqlDbType type, int size, string? value)
        {
            var trimmedValue = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            command.Parameters.Add(name, type, size).Value = trimmedValue is null ? DBNull.Value : trimmedValue;
        }

        private static Motorista MapMotorista(SqlDataReader reader)
        {
            return new Motorista
            {
                Id = reader.GetInt32("Id"),
                Nome = reader.GetString("Nome"),
                Cnh = reader.GetString("Cnh"),
                Ativo = reader.GetBoolean("Ativo"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
