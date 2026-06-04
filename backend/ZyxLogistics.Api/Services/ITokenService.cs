using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiraEm) GenerateToken(AuthUsuario usuario, IReadOnlyList<Permissao> permissoes);
    }
}
