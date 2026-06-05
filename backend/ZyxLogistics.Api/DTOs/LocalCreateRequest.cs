using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class LocalCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;
    }
}
