using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface ITransportadoraRepository
    {
        Task<IReadOnlyList<Transportadora>> ListarAsync(TransportadoraFilterRequest filter);
        Task<Transportadora?> ObterPorIdAsync(int id);
        Task<int> InserirAsync(TransportadoraCreateRequest request);
        Task<bool> AtualizarAsync(int id, TransportadoraUpdateRequest request);
        Task<bool> ExcluirAsync(int id);
    }
}
