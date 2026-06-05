using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/operacoes")]
    [Authorize(Policy = "operacoes.visualizar")]
    public class OperacoesController : ControllerBase
    {
        private readonly IOperacaoRepository _operacaoRepository;

        public OperacoesController(IOperacaoRepository operacaoRepository)
        {
            _operacaoRepository = operacaoRepository;
        }

        [HttpGet("{operacao}/abas")]
        public async Task<IActionResult> ListarAbas(string operacao, [FromQuery] string? busca)
        {
            try
            {
                var abas = await _operacaoRepository.ListarAbasAsync(operacao, busca);
                return Ok(abas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
