using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.DTOs
{
    public class AuthUsuarioResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PerfilId { get; set; }
        public string PerfilDescricao { get; set; } = string.Empty;
        public bool PrimeiroAcesso { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public IReadOnlyList<Permissao> Permissoes { get; set; } = [];
    }
}
