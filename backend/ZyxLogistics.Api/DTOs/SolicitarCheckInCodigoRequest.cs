using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class SolicitarCheckInCodigoRequest
    {
        [Required]
        [StringLength(30)]
        public string Cnh { get; set; } = string.Empty;
    }
}
