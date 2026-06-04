using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class ProdutoFilterRequest
    {
        [StringLength(150)]
        public string? Descricao { get; set; }
    }
}
