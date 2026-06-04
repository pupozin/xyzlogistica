using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class TransportadoraUpdateRequest
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Cnpj { get; set; } = string.Empty;
    }
}
