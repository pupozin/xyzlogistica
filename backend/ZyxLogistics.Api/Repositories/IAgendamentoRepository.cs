using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IAgendamentoRepository
    {
        Task<IReadOnlyList<Agendamento>> ListarAsync(DateTime data, int operacaoId);
        Task<Agendamento?> ObterPorIdAsync(int id);
        Task<IReadOnlyList<HorarioDisponivel>> ListarHorariosDisponiveisAsync(DateTime data);
        Task<IReadOnlyList<VeiculoDisponivel>> ListarVeiculosDisponiveisAsync(int transportadoraId);
        Task<int> InserirAsync(AgendamentoCreateRequest request);
        Task<bool> AtualizarAsync(int id, AgendamentoUpdateRequest request);
        Task<bool> CancelarAsync(int id);
        Task<bool> EnviarDocaAsync(int id, EnviarDocaRequest request);
        Task<bool> FinalizarAsync(int id);
    }
}
