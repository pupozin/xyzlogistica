using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/configuracao-agendamento")]
    public class ConfiguracaoAgendamentoController : ControllerBase
    {
        private readonly IConfiguracaoAgendamentoRepository _configuracaoAgendamentoRepository;

        public ConfiguracaoAgendamentoController(IConfiguracaoAgendamentoRepository configuracaoAgendamentoRepository)
        {
            _configuracaoAgendamentoRepository = configuracaoAgendamentoRepository;
        }

        [HttpGet("ativa")]
        public async Task<IActionResult> ObterAtiva()
        {
            var configuracao = await _configuracaoAgendamentoRepository.ObterAtivaAsync();

            if (configuracao is null)
            {
                return NotFound();
            }

            return Ok(configuracao);
        }

        [HttpPut("ativa")]
        public async Task<IActionResult> AtualizarAtiva(ConfiguracaoAgendamentoUpdateRequest request)
        {
            if (request.HoraInicio >= request.HoraFim)
            {
                return BadRequest(new { message = "A hora inicial deve ser menor que a hora final." });
            }

            try
            {
                var configuracao = await _configuracaoAgendamentoRepository.AtualizarAsync(request);
                return Ok(configuracao);
            }
            catch (SqlException ex) when (ex.Number == 50003)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
