using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentosController : ControllerBase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;

        public AgendamentosController(IAgendamentoRepository agendamentoRepository)
        {
            _agendamentoRepository = agendamentoRepository;
        }

        [HttpGet]
        [Authorize(Policy = "agendamentos.visualizar")]
        public async Task<IActionResult> Listar([FromQuery] AgendamentoFilterRequest filter)
        {
            try
            {
                var agendamentos = await _agendamentoRepository.ListarAsync(filter.Data, filter.OperacaoId);
                return Ok(agendamentos);
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "agendamentos.visualizar")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);

            if (agendamento is null)
            {
                return NotFound();
            }

            return Ok(agendamento);
        }

        [HttpGet("horarios-disponiveis")]
        [Authorize(Policy = "agendamentos.visualizar")]
        public async Task<IActionResult> ListarHorariosDisponiveis([FromQuery] DateTime data)
        {
            var horarios = await _agendamentoRepository.ListarHorariosDisponiveisAsync(data);
            return Ok(horarios);
        }

        [HttpGet("motoristas-disponiveis")]
        [Authorize(Policy = "agendamentos.visualizar")]
        public async Task<IActionResult> ListarMotoristasDisponiveis()
        {
            var motoristas = await _agendamentoRepository.ListarMotoristasDisponiveisAsync();
            return Ok(motoristas);
        }

        [HttpGet("veiculos-disponiveis")]
        [Authorize(Policy = "agendamentos.visualizar")]
        public async Task<IActionResult> ListarVeiculosDisponiveis([FromQuery] int transportadoraId)
        {
            if (transportadoraId <= 0)
            {
                return BadRequest(new { message = "Transportadora e obrigatoria para listar veiculos." });
            }

            try
            {
                var veiculos = await _agendamentoRepository.ListarVeiculosDisponiveisAsync(transportadoraId);
                return Ok(veiculos);
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "agendamentos.criar")]
        public async Task<IActionResult> Inserir(AgendamentoCreateRequest request)
        {
            try
            {
                var id = await _agendamentoRepository.InserirAsync(request);
                var agendamento = await _agendamentoRepository.ObterPorIdAsync(id);

                return CreatedAtAction(nameof(ObterPorId), new { id }, agendamento);
            }
            catch (SqlException ex) when (ex.Number is 50004 or 50008)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "agendamentos.editar")]
        public async Task<IActionResult> Atualizar(int id, AgendamentoUpdateRequest request)
        {
            try
            {
                var atualizado = await _agendamentoRepository.AtualizarAsync(id, request);

                if (!atualizado)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (ex.Number is 50004 or 50008)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/cancelar")]
        [Authorize(Policy = "agendamentos.cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var cancelado = await _agendamentoRepository.CancelarAsync(id);

                if (!cancelado)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/enviar-doca")]
        [Authorize(Policy = "operacoes.enviar_doca")]
        public async Task<IActionResult> EnviarDoca(int id, EnviarDocaRequest request)
        {
            try
            {
                var enviado = await _agendamentoRepository.EnviarDocaAsync(id, request);

                if (!enviado)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/finalizar")]
        [Authorize(Policy = "operacoes.finalizar")]
        public async Task<IActionResult> Finalizar(int id)
        {
            try
            {
                var finalizado = await _agendamentoRepository.FinalizarAsync(id);

                if (!finalizado)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
