using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class ConfirmarCheckInRequest
    {
        [Required]
        [StringLength(30)]
        public string Cnh { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Codigo { get; set; } = string.Empty;
    }
}
