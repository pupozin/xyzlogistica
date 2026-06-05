namespace ZyxLogistics.Api.Models
{
    public class Motorista
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cnh { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
