using ZyxLogistics.Api.DTOs;

namespace ZyxLogistics.Api.Repositories
{
    public interface ICheckInRepository
    {
        Task<CheckInCodigoResponse> SolicitarCodigoAsync(SolicitarCheckInCodigoRequest request);
        Task<bool> ConfirmarAsync(ConfirmarCheckInRequest request);
    }
}
