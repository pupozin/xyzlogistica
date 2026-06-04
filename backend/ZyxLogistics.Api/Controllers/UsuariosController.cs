using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] UsuarioFilterRequest filter)
        {
            var usuarios = await _usuarioRepository.ListarAsync(filter);
            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

            if (usuario is null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Inserir(UsuarioCreateRequest request)
        {
            try
            {
                var id = await _usuarioRepository.InserirAsync(request);
                var usuario = await _usuarioRepository.ObterPorIdAsync(id);

                return CreatedAtAction(nameof(ObterPorId), new { id }, usuario);
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Atualizar(int id, UsuarioUpdateRequest request)
        {
            try
            {
                var atualizado = await _usuarioRepository.AtualizarAsync(id, request);

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
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _usuarioRepository.ExcluirAsync(id);

            if (!excluido)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
