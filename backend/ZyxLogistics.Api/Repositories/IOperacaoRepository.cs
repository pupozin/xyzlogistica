using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public interface IOperacaoRepository
    {
        Task<IReadOnlyList<OperacaoAba>> ListarAbasAsync(string operacao, string? busca);
    }
}
