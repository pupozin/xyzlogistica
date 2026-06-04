using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IPerfilRepository
    {
        Task<IReadOnlyList<Perfil>> ListarPerfisAsync();
        Task<Perfil?> ObterPerfilPorIdAsync(int id);
        Task<Perfil> InserirPerfilAsync(PerfilCreateRequest request);
        Task<Perfil?> AtualizarPerfilAsync(int id, PerfilUpdateRequest request);
        Task<bool> ExcluirPerfilAsync(int id);
        Task<IReadOnlyList<Permissao>> ListarPermissoesAsync();
        Task<IReadOnlyList<Permissao>> ListarPermissoesPorPerfilAsync(int perfilId);
        Task<IReadOnlyList<Permissao>> AtualizarPermissoesAsync(int perfilId, PerfilPermissoesUpdateRequest request);
    }
}
