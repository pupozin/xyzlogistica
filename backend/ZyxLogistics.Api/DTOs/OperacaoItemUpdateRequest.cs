using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class OperacaoItemUpdateRequest
    {
        [Range(0.001, 999999999999999.999)]
        public decimal Quantidade { get; set; }
    }
}
