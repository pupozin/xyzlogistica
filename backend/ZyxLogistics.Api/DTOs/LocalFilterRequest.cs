using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class LocalFilterRequest
    {
        [StringLength(100)]
        public string? Descricao { get; set; }
    }
}
