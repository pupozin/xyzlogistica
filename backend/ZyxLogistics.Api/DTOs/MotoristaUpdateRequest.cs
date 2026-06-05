using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class MotoristaUpdateRequest
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Cnh { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;
    }
}
