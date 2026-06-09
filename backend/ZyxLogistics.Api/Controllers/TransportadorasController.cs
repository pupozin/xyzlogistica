using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportadorasController : ControllerBase
    {
        private readonly ITransportadoraRepository _transportadoraRepository;

        public TransportadorasController(ITransportadoraRepository transportadoraRepository)
        {
            _transportadoraRepository = transportadoraRepository;
        }

        [HttpGet]
        [Authorize(Policy = "transportadoras.visualizar")]
        public async Task<IActionResult> Listar([FromQuery] TransportadoraFilterRequest filter)
        {
            var transportadoras = await _transportadoraRepository.ListarAsync(filter);
            return Ok(transportadoras);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "transportadoras.visualizar")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var transportadora = await _transportadoraRepository.ObterPorIdAsync(id);

            if (transportadora is null)
            {
                return NotFound();
            }

            return Ok(transportadora);
        }

        [HttpPost]
        [Authorize(Policy = "transportadoras.criar")]
        public async Task<IActionResult> Inserir(TransportadoraCreateRequest request)
        {
            try
            {
                var id = await _transportadoraRepository.InserirAsync(request);
                var transportadora = await _transportadoraRepository.ObterPorIdAsync(id);

                return CreatedAtAction(nameof(ObterPorId), new { id }, transportadora);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "transportadoras.editar")]
        public async Task<IActionResult> Atualizar(int id, TransportadoraUpdateRequest request)
        {
            try
            {
                var atualizado = await _transportadoraRepository.AtualizarAsync(id, request);

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
        [Authorize(Policy = "transportadoras.excluir")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _transportadoraRepository.ExcluirAsync(id);

            if (!excluido)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
