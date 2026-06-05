using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class OperacaoItemCreateRequest
    {
        [Range(1, int.MaxValue)]
        public int ProdutoId { get; set; }

        [Range(0.001, 999999999999999.999)]
        public decimal Quantidade { get; set; }
    }
}
