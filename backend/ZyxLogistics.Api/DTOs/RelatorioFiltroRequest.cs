using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class RelatorioFiltroRequest
    {
        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        public int? OperacaoId { get; set; }
        public int? StatusId { get; set; }
        public string? Produto { get; set; }
    }
}
