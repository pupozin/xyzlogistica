using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class ProdutoCreateRequest
    {
        [Required]
        [StringLength(150)]
        public string Descricao { get; set; } = string.Empty;
    }
}
