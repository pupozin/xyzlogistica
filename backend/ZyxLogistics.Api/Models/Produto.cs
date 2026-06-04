namespace ZyxLogistics.Api.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
