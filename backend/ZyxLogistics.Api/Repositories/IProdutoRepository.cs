using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IProdutoRepository
    {
        Task<IReadOnlyList<Produto>> ListarAsync(ProdutoFilterRequest filter);
        Task<Produto?> ObterPorIdAsync(int id);
        Task<int> InserirAsync(ProdutoCreateRequest request);
        Task<bool> AtualizarAsync(int id, ProdutoUpdateRequest request);
        Task<bool> ExcluirAsync(int id);
    }
}
