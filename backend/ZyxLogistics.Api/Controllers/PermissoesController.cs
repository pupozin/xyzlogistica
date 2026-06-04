using Microsoft.AspNetCore.Mvc;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/permissoes")]
    public class PermissoesController : ControllerBase
    {
        private readonly IPerfilRepository _perfilRepository;

        public PermissoesController(IPerfilRepository perfilRepository)
        {
            _perfilRepository = perfilRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPermissoes()
        {
            var permissoes = await _perfilRepository.ListarPermissoesAsync();
            return Ok(permissoes);
        }
    }
}
