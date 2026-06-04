using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class ProdutoUpdateRequest
    {
        [Required]
        [StringLength(150)]
        public string Descricao { get; set; } = string.Empty;
    }
}
