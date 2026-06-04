using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IVeiculoRepository
    {
        Task<IReadOnlyList<Veiculo>> ListarAsync(VeiculoFilterRequest filter);
        Task<Veiculo?> ObterPorIdAsync(int id);
        Task<int> InserirAsync(VeiculoCreateRequest request);
        Task<bool> AtualizarAsync(int id, VeiculoUpdateRequest request);
        Task<bool> ExcluirAsync(int id);
    }
}
