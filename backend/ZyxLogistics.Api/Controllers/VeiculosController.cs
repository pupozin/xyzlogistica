using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly IVeiculoRepository _veiculoRepository;

        public VeiculosController(IVeiculoRepository veiculoRepository)
        {
            _veiculoRepository = veiculoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] VeiculoFilterRequest filter)
        {
            var veiculos = await _veiculoRepository.ListarAsync(filter);
            return Ok(veiculos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(id);

            if (veiculo is null)
            {
                return NotFound();
            }

            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Inserir(VeiculoCreateRequest request)
        {
            try
            {
                var id = await _veiculoRepository.InserirAsync(request);
                var veiculo = await _veiculoRepository.ObterPorIdAsync(id);

                return CreatedAtAction(nameof(ObterPorId), new { id }, veiculo);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50002)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, VeiculoUpdateRequest request)
        {
            try
            {
                var atualizado = await _veiculoRepository.AtualizarAsync(id, request);

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
            catch (SqlException ex) when (ex.Number == 50002)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _veiculoRepository.ExcluirAsync(id);

            if (!excluido)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
