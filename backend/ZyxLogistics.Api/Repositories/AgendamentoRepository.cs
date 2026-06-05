using System.Data;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AgendamentoRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Agendamento>> ListarAsync(DateTime data, int operacaoId)
        {
            var agendamentos = new List<Agendamento>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Agendamento_Listar");
            command.Parameters.Add("@Data", SqlDbType.Date).Value = data.Date;
            command.Parameters.Add("@OperacaoId", SqlDbType.Int).Value = operacaoId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                agendamentos.Add(MapAgendamento(reader));
            }

            return agendamentos;
        }

        public async Task<Agendamento?> ObterPorIdAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Agendamento_ObterPorId");
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapAgendamento(reader);
        }

        public async Task<IReadOnlyList<HorarioDisponivel>> ListarHorariosDisponiveisAsync(DateTime data)
        {
            var horarios = new List<HorarioDisponivel>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Agendamento_ListarHorariosDisponiveis");
            command.Parameters.Add("@Data", SqlDbType.Date).Value = data.Date;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                horarios.Add(new HorarioDisponivel
                {
                    DataHora = reader.GetDateTime("DataHora"),
                    Horario = reader.GetString("Horario")
                });
            }

            return horarios;
        }

        public async Task<IReadOnlyList<VeiculoDisponivel>> ListarVeiculosDisponiveisAsync(int transportadoraId)
        {
            var veiculos = new List<VeiculoDisponivel>();

            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Agendamento_ListarVeiculosDisponiveis");
            command.Parameters.Add("@TransportadoraId", SqlDbType.Int).Value = transportadoraId;

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                veiculos.Add(new VeiculoDisponivel
                {
                    Id = reader.GetInt32("Id"),
                    Placa = reader.GetString("Placa"),
                    TipoVeiculo = reader.GetString("TipoVeiculo"),
                    TransportadoraId = reader.GetInt32("TransportadoraId"),
                    TransportadoraNome = reader.GetString("TransportadoraNome")
                });
            }

            return veiculos;
        }

        public async Task<int> InserirAsync(AgendamentoCreateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Agendamento_Inserir");
            command.Parameters.Add("@OperacaoId", SqlDbType.Int).Value = request.OperacaoId;
            command.Parameters.Add("@TransportadoraId", SqlDbType.Int).Value = request.TransportadoraId;
            command.Parameters.Add("@VeiculoId", SqlDbType.Int).Value = request.VeiculoId;
            command.Parameters.Add("@MotoristaId", SqlDbType.Int).Value = request.MotoristaId;
            command.Parameters.Add("@DataHoraAgendada", SqlDbType.DateTime2).Value = request.DataHoraAgendada;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> AtualizarAsync(int id, AgendamentoUpdateRequest request)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Agendamento_Atualizar");
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@OperacaoId", SqlDbType.Int).Value = request.OperacaoId;
            command.Parameters.Add("@TransportadoraId", SqlDbType.Int).Value = request.TransportadoraId;
            command.Parameters.Add("@VeiculoId", SqlDbType.Int).Value = request.VeiculoId;
            command.Parameters.Add("@MotoristaId", SqlDbType.Int).Value = request.MotoristaId;
            command.Parameters.Add("@DataHoraAgendada", SqlDbType.DateTime2).Value = request.DataHoraAgendada;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> CancelarAsync(int id)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = CreateProcedureCommand(connection, "dbo.sp_Agendamento_Cancelar");
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
