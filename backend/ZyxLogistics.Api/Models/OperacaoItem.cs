namespace ZyxLogistics.Api.Models
{
    public class OperacaoItem
    {
        public int Id { get; set; }
        public int AgendamentoId { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoDescricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
