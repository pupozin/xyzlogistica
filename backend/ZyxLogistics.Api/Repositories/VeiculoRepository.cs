using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public VeiculoRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Veiculo>> ListarAsync(VeiculoFilterRequest filter)
        {
            var veiculos = new List<Veiculo>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Veiculo_Listar");
            AddNullableStringParameter(command, "@Placa", SqlDbType.VarChar, 10, filter.Placa);
            AddNullableStringParameter(command, "@TipoVeiculo", SqlDbType.NVarChar, 80, filter.TipoVeiculo);
            command.Parameters.Add("@TransportadoraId", SqlDbType.Int).Value = filter.TransportadoraId.HasValue ? filter.TransportadoraId.Value : DBNull.Value;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                veiculos.Add(MapVeiculo(reader));
            }

            return veiculos;
        }

        public async Task<Veiculo?> ObterPorIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Veiculo_ObterPorId");
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapVeiculo(reader);
        }

        public async Task<int> InserirAsync(VeiculoCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Veiculo_Inserir");
            AddVeiculoParameters(command, request.Placa, request.TipoVeiculo, request.TransportadoraId);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> AtualizarAsync(int id, VeiculoUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Veiculo_Atualizar");
            command.Parameters.AddWithValue("@Id", id);
            AddVeiculoParameters(command, request.Placa, request.TipoVeiculo, request.TransportadoraId);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Veiculo_Excluir");
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

        private static void AddVeiculoParameters(SqlCommand command, string placa, string tipoVeiculo, int transportadoraId)
        {
            command.Parameters.Add("@Placa", SqlDbType.VarChar, 10).Value = placa.Trim().ToUpperInvariant();
            command.Parameters.Add("@TipoVeiculo", SqlDbType.NVarChar, 80).Value = tipoVeiculo.Trim();
            command.Parameters.Add("@TransportadoraId", SqlDbType.Int).Value = transportadoraId;
        }

        private static void AddNullableStringParameter(SqlCommand command, string name, SqlDbType type, int size, string? value)
        {
            var trimmedValue = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            command.Parameters.Add(name, type, size).Value = trimmedValue is null ? DBNull.Value : trimmedValue;
        }

        private static Veiculo MapVeiculo(SqlDataReader reader)
        {
            return new Veiculo
            {
                Id = reader.GetInt32("Id"),
                Placa = reader.GetString("Placa"),
                TipoVeiculo = reader.GetString("TipoVeiculo"),
                TransportadoraId = reader.GetInt32("TransportadoraId"),
                TransportadoraNome = reader.GetString("TransportadoraNome"),
                Ativo = reader.GetBoolean("Ativo"),
                CriadoEm = reader.GetDateTime("CriadoEm"),
                AtualizadoEm = reader.IsDBNull("AtualizadoEm") ? null : reader.GetDateTime("AtualizadoEm")
            };
        }
    }
}
