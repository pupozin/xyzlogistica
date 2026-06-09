using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IRelatorioRepository
    {
        Task<RelatorioExcel> GerarAsync(string tipo, RelatorioFiltroRequest filtro);
    }
}
