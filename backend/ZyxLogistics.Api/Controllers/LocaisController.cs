using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocaisController : ControllerBase
    {
        private readonly ILocalRepository _localRepository;

        public LocaisController(ILocalRepository localRepository)
        {
            _localRepository = localRepository;
        }

        [HttpGet]
        [Authorize(Policy = "locais.visualizar")]
        public async Task<IActionResult> Listar([FromQuery] LocalFilterRequest filter)
        {
            var locais = await _localRepository.ListarAsync(filter);
            return Ok(locais);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "locais.visualizar")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var local = await _localRepository.ObterPorIdAsync(id);

            if (local is null)
            {
                return NotFound();
            }

            return Ok(local);
        }

        [HttpPost]
        [Authorize(Policy = "locais.criar")]
        public async Task<IActionResult> Inserir(LocalCreateRequest request)
        {
            try
            {
                var id = await _localRepository.InserirAsync(request);
                var local = await _localRepository.ObterPorIdAsync(id);

                return CreatedAtAction(nameof(ObterPorId), new { id }, local);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "locais.editar")]
        public async Task<IActionResult> Atualizar(int id, LocalUpdateRequest request)
        {
            try
            {
                var atualizado = await _localRepository.AtualizarAsync(id, request);

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
        [Authorize(Policy = "locais.excluir")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var excluido = await _localRepository.ExcluirAsync(id);

                if (!excluido)
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
