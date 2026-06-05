using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface ILocalRepository
    {
        Task<IReadOnlyList<Local>> ListarAsync(LocalFilterRequest filter);
        Task<Local?> ObterPorIdAsync(int id);
        Task<int> InserirAsync(LocalCreateRequest request);
        Task<bool> AtualizarAsync(int id, LocalUpdateRequest request);
        Task<bool> ExcluirAsync(int id);
    }
}
