using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class UsuarioUpdateRequest
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int PerfilId { get; set; }
    }
}
