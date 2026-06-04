using ZyxLogistics.Api.DTOs;

namespace ZyxLogistics.Api.Services
{
    public interface IAuthService
    {
        Task<PrimeiroAcessoResponse?> VerificarPrimeiroAcessoAsync(string email);
        Task<AuthUsuarioResponse?> DefinirSenhaPrimeiroAcessoAsync(DefinirSenhaPrimeiroAcessoRequest request);
        Task<AuthUsuarioResponse?> LoginAsync(LoginRequest request);
    }
}
