using Microsoft.AspNetCore.Mvc;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Services;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("primeiro-acesso")]
        public async Task<IActionResult> VerificarPrimeiroAcesso([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email e obrigatorio." });
            }

            var response = await _authService.VerificarPrimeiroAcessoAsync(email);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost("primeiro-acesso")]
        public async Task<IActionResult> DefinirSenhaPrimeiroAcesso(DefinirSenhaPrimeiroAcessoRequest request)
        {
            var response = await _authService.DefinirSenhaPrimeiroAcessoAsync(request);

            if (response is null)
            {
                return BadRequest(new { message = "Usuario nao encontrado ou primeiro acesso ja realizado." });
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (response is null)
            {
                return Unauthorized(new { message = "Email ou senha invalidos." });
            }

            return Ok(response);
        }
    }
}
