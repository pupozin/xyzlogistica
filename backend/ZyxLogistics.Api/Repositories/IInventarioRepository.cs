using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IInventarioRepository
    {
        Task<IReadOnlyList<InventarioItem>> ListarAsync(InventarioFilterRequest filter);
    }
}
