using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IAuthRepository
    {
        Task<AuthUsuario?> ObterUsuarioPorEmailAsync(string email);
        Task<bool> DefinirSenhaPrimeiroAcessoAsync(string email, string senhaHash);
        Task<IReadOnlyList<Permissao>> ListarPermissoesAsync(int usuarioId);
    }
}
