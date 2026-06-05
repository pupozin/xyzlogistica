using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/agendamentos/{agendamentoId:int}/itens")]
    public class AgendamentoItensController : ControllerBase
    {
        private readonly IOperacaoItemRepository _operacaoItemRepository;

        public AgendamentoItensController(IOperacaoItemRepository operacaoItemRepository)
        {
            _operacaoItemRepository = operacaoItemRepository;
        }

        [HttpGet]
        [Authorize(Policy = "operacoes.visualizar")]
        public async Task<IActionResult> Listar(int agendamentoId)
        {
            var itens = await _operacaoItemRepository.ListarPorAgendamentoAsync(agendamentoId);
            return Ok(itens);
        }

        [HttpPost]
        [Authorize(Policy = "operacoes.enviar_doca")]
        public async Task<IActionResult> Inserir(int agendamentoId, OperacaoItemCreateRequest request)
        {
            try
            {
                var id = await _operacaoItemRepository.InserirAsync(agendamentoId, request);
                return CreatedAtAction(nameof(Listar), new { agendamentoId }, new { id });
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "operacoes.enviar_doca")]
        public async Task<IActionResult> Atualizar(int agendamentoId, int id, OperacaoItemUpdateRequest request)
        {
            try
            {
                var atualizado = await _operacaoItemRepository.AtualizarAsync(agendamentoId, id, request);

                if (!atualizado)
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

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "operacoes.enviar_doca")]
        public async Task<IActionResult> Excluir(int agendamentoId, int id)
        {
            try
            {
                var excluido = await _operacaoItemRepository.ExcluirAsync(agendamentoId, id);

                if (!excluido)
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
    }
}
