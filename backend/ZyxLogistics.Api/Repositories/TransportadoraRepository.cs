using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class TransportadoraRepository : ITransportadoraRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TransportadoraRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Transportadora>> ListarAsync(TransportadoraFilterRequest filter)
        {
            var transportadoras = new List<Transportadora>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Transportadora_Listar");
            AddNullableStringParameter(command, "@Nome", SqlDbType.NVarChar, 150, filter.Nome);
            AddNullableStringParameter(command, "@Cnpj", SqlDbType.VarChar, 20, filter.Cnpj);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transportadoras.Add(MapTransportadora(reader));
            }

            return transportadoras;
        }

        public async Task<Transportadora?> ObterPorIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Transportadora_ObterPorId");
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapTransportadora(reader);
        }

        public async Task<int> InserirAsync(TransportadoraCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Transportadora_Inserir");
            AddTransportadoraParameters(command, request.Nome, request.Cnpj);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> AtualizarAsync(int id, TransportadoraUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Transportadora_Atualizar");
            command.Parameters.AddWithValue("@Id", id);
            AddTransportadoraParameters(command, request.Nome, request.Cnpj);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Transportadora_Excluir");
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

        private static void AddTransportadoraParameters(SqlCommand command, string nome, string cnpj)
        {
            command.Parameters.Add("@Nome", SqlDbType.NVarChar, 150).Value = nome.Trim();
            command.Parameters.Add("@Cnpj", SqlDbType.VarChar, 20).Value = cnpj.Trim();
        }

        private static void AddNullableStringParameter(SqlCommand command, string name, SqlDbType type, int size, string? value)
        {
            var trimmedValue = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            command.Parameters.Add(name, type, size).Value = trimmedValue is null ? DBNull.Value : trimmedValue;
        }

        private static Transportadora MapTransportadora(SqlDataReader reader)
        {
            return new Transportadora
            {
                Id = reader.GetInt32("Id"),
                Nome = reader.GetString("Nome"),
                Cnpj = reader.GetString("Cnpj"),
                Ativo = reader.GetBoolean("Ativo"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
