using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<Usuario>> ListarAsync(UsuarioFilterRequest filter);
        Task<Usuario?> ObterPorIdAsync(int id);
        Task<int> InserirAsync(UsuarioCreateRequest request);
        Task<bool> AtualizarAsync(int id, UsuarioUpdateRequest request);
        Task<bool> ExcluirAsync(int id);
    }
}
