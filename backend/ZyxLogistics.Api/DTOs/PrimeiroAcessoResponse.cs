namespace ZyxLogistics.Api.DTOs
{
    public class PrimeiroAcessoResponse
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool PrimeiroAcesso { get; set; }
    }
}
