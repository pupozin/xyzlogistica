using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "inventario.visualizar")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioRepository _inventarioRepository;

        public InventarioController(IInventarioRepository inventarioRepository)
        {
            _inventarioRepository = inventarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] InventarioFilterRequest filter)
        {
            var itens = await _inventarioRepository.ListarAsync(filter);
            return Ok(itens);
        }
    }
}
