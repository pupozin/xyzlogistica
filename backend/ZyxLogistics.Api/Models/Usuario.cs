namespace ZyxLogistics.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PerfilId { get; set; }
        public string PerfilDescricao { get; set; } = string.Empty;
        public bool PrimeiroAcesso { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
