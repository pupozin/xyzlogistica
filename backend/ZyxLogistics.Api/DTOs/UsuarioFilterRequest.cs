using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class UsuarioFilterRequest
    {
        [StringLength(150)]
        public string? Nome { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        public int? PerfilId { get; set; }
    }
}
