using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private readonly IRelatorioRepository _relatorioRepository;

        public RelatoriosController(IRelatorioRepository relatorioRepository)
        {
            _relatorioRepository = relatorioRepository;
        }

        [HttpGet("{tipo}/excel")]
        [Authorize(Policy = "relatorios.visualizar")]
        public async Task<IActionResult> GerarExcel(string tipo, [FromQuery] RelatorioFiltroRequest filtro)
        {
            try
            {
                var relatorio = await _relatorioRepository.GerarAsync(tipo, filtro);
                return File(relatorio.Content, ExcelContentType, relatorio.FileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
