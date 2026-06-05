using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Repositories;

namespace ZyxLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/checkin")]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInRepository _checkInRepository;

        public CheckInController(ICheckInRepository checkInRepository)
        {
            _checkInRepository = checkInRepository;
        }

        [HttpPost("solicitar-codigo")]
        public async Task<IActionResult> SolicitarCodigo(SolicitarCheckInCodigoRequest request)
        {
            try
            {
                var response = await _checkInRepository.SolicitarCodigoAsync(request);
                return Ok(response);
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("confirmar")]
        public async Task<IActionResult> Confirmar(ConfirmarCheckInRequest request)
        {
            try
            {
                var confirmado = await _checkInRepository.ConfirmarAsync(request);

                if (!confirmado)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 50004)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50007)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
