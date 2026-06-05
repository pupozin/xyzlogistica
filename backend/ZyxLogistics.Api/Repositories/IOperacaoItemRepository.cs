using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IOperacaoItemRepository
    {
        Task<IReadOnlyList<OperacaoItem>> ListarPorAgendamentoAsync(int agendamentoId);
        Task<int> InserirAsync(int agendamentoId, OperacaoItemCreateRequest request);
        Task<bool> AtualizarAsync(int agendamentoId, int id, OperacaoItemUpdateRequest request);
        Task<bool> ExcluirAsync(int agendamentoId, int id);
    }
}
