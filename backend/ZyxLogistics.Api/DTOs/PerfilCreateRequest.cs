using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class PerfilCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;

        public List<int> PermissaoIds { get; set; } = [];
    }
}
