namespace ZyxLogistics.Api.Models
{
    public class InventarioItem
    {
        public int ProdutoId { get; set; }
        public string ProdutoDescricao { get; set; } = string.Empty;
        public decimal QuantidadeAtual { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
