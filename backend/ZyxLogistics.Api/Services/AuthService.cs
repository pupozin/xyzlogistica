using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthService(IAuthRepository authRepository, IPasswordService passwordService, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<PrimeiroAcessoResponse?> VerificarPrimeiroAcessoAsync(string email)
        {
            var usuario = await _authRepository.ObterUsuarioPorEmailAsync(email);

            if (usuario is null)
            {
                return null;
            }

            return new PrimeiroAcessoResponse
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                PrimeiroAcesso = usuario.PrimeiroAcesso
            };
        }

        public async Task<AuthUsuarioResponse?> DefinirSenhaPrimeiroAcessoAsync(DefinirSenhaPrimeiroAcessoRequest request)
        {
            var usuario = await _authRepository.ObterUsuarioPorEmailAsync(request.Email);

            if (usuario is null || !usuario.PrimeiroAcesso)
            {
                return null;
            }

            var senhaHash = _passwordService.HashPassword(request.Senha);
            var atualizou = await _authRepository.DefinirSenhaPrimeiroAcessoAsync(request.Email, senhaHash);

            if (!atualizou)
            {
                return null;
            }

            var usuarioAtualizado = await _authRepository.ObterUsuarioPorEmailAsync(request.Email);
            return usuarioAtualizado is null ? null : await CreateResponseAsync(usuarioAtualizado);
        }

        public async Task<AuthUsuarioResponse?> LoginAsync(LoginRequest request)
        {
            var usuario = await _authRepository.ObterUsuarioPorEmailAsync(request.Email);

            if (usuario is null || usuario.Senha is null)
            {
                return null;
            }

            if (!_passwordService.VerifyPassword(request.Senha, usuario.Senha))
            {
                return null;
            }

            return await CreateResponseAsync(usuario);
        }

        private async Task<AuthUsuarioResponse> CreateResponseAsync(AuthUsuario usuario)
        {
            var permissoes = await _authRepository.ListarPermissoesAsync(usuario.Id);
            var token = _tokenService.GenerateToken(usuario, permissoes);

            return new AuthUsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                PerfilId = usuario.PerfilId,
                PerfilDescricao = usuario.PerfilDescricao,
                PrimeiroAcesso = usuario.PrimeiroAcesso,
                Token = token.Token,
                ExpiraEm = token.ExpiraEm,
                Permissoes = permissoes
            };
        }
    }
}
