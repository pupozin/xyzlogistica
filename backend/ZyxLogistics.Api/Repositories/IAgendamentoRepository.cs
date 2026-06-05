using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IAgendamentoRepository
    {
        Task<IReadOnlyList<Agendamento>> ListarAsync(DateTime data);
        Task<Agendamento?> ObterPorIdAsync(int id);
        Task<IReadOnlyList<HorarioDisponivel>> ListarHorariosDisponiveisAsync(DateTime data);
        Task<IReadOnlyList<VeiculoDisponivel>> ListarVeiculosDisponiveisAsync(int transportadoraId);
        Task<int> InserirAsync(AgendamentoCreateRequest request);
        Task<bool> CancelarAsync(int id);
    }
}
