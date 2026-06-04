using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/perfis")]
    public class PerfisController : ControllerBase
    {
        private readonly IPerfilRepository _perfilRepository;

        public PerfisController(IPerfilRepository perfilRepository)
        {
            _perfilRepository = perfilRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPerfis()
        {
            var perfis = await _perfilRepository.ListarPerfisAsync();
            return Ok(perfis);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPerfilPorId(int id)
        {
            var perfil = await _perfilRepository.ObterPerfilPorIdAsync(id);

            if (perfil is null)
            {
                return NotFound();
            }

            return Ok(perfil);
        }

        [HttpPost]
        public async Task<IActionResult> InserirPerfil(PerfilCreateRequest request)
        {
            try
            {
                var perfil = await _perfilRepository.InserirPerfilAsync(request);
                return CreatedAtAction(nameof(ObterPerfilPorId), new { id = perfil.Id }, perfil);
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50005)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarPerfil(int id, PerfilUpdateRequest request)
        {
            try
            {
                var perfil = await _perfilRepository.AtualizarPerfilAsync(id, request);

                if (perfil is null)
                {
                    return NotFound();
                }

                return Ok(perfil);
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50005)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> ExcluirPerfil(int id)
        {
            try
            {
                var excluido = await _perfilRepository.ExcluirPerfilAsync(id);

                if (!excluido)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 50006)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{perfilId:int}/permissoes")]
        public async Task<IActionResult> ListarPermissoesPorPerfil(int perfilId)
        {
            var permissoes = await _perfilRepository.ListarPermissoesPorPerfilAsync(perfilId);
            return Ok(permissoes);
        }

        [HttpPut("{perfilId:int}/permissoes")]
        public async Task<IActionResult> AtualizarPermissoes(int perfilId, PerfilPermissoesUpdateRequest request)
        {
            try
            {
                var permissoes = await _perfilRepository.AtualizarPermissoesAsync(perfilId, request);
                return Ok(permissoes);
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
