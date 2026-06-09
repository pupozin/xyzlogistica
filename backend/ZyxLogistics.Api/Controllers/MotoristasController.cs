using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotoristasController : ControllerBase
    {
        private readonly IMotoristaRepository _motoristaRepository;

        public MotoristasController(IMotoristaRepository motoristaRepository)
        {
            _motoristaRepository = motoristaRepository;
        }

        [HttpGet]
        [Authorize(Policy = "motoristas.visualizar")]
        public async Task<IActionResult> Listar([FromQuery] MotoristaFilterRequest filter)
        {
            var motoristas = await _motoristaRepository.ListarAsync(filter);
            return Ok(motoristas);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "motoristas.visualizar")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var motorista = await _motoristaRepository.ObterPorIdAsync(id);

            if (motorista is null)
            {
                return NotFound();
            }

            return Ok(motorista);
        }

        [HttpPost]
        [Authorize(Policy = "motoristas.criar")]
        public async Task<IActionResult> Inserir(MotoristaCreateRequest request)
        {
            try
            {
                var id = await _motoristaRepository.InserirAsync(request);
                var motorista = await _motoristaRepository.ObterPorIdAsync(id);

                return CreatedAtAction(nameof(ObterPorId), new { id }, motorista);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "motoristas.editar")]
        public async Task<IActionResult> Atualizar(int id, MotoristaUpdateRequest request)
        {
            try
            {
                var atualizado = await _motoristaRepository.AtualizarAsync(id, request);

                if (!atualizado)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "motoristas.excluir")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _motoristaRepository.ExcluirAsync(id);

            if (!excluido)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
