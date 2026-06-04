using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IMotoristaRepository
    {
        Task<IReadOnlyList<Motorista>> ListarAsync(MotoristaFilterRequest filter);
        Task<Motorista?> ObterPorIdAsync(int id);
        Task<int> InserirAsync(MotoristaCreateRequest request);
        Task<bool> AtualizarAsync(int id, MotoristaUpdateRequest request);
        Task<bool> ExcluirAsync(int id);
    }
}
