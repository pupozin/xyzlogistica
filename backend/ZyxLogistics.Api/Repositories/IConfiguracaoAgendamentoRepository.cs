using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IConfiguracaoAgendamentoRepository
    {
        Task<ConfiguracaoAgendamento?> ObterAtivaAsync();
        Task<ConfiguracaoAgendamento> AtualizarAsync(ConfiguracaoAgendamentoUpdateRequest request);
    }
}
