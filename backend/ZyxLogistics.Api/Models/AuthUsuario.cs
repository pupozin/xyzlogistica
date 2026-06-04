namespace ZyxLogistics.Api.Models
{
    public class AuthUsuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Senha { get; set; }
        public int PerfilId { get; set; }
        public string PerfilDescricao { get; set; } = string.Empty;
        public bool PrimeiroAcesso { get; set; }
        public bool Ativo { get; set; }
    }
}
