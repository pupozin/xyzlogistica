using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class InventarioFilterRequest
    {
        [StringLength(150)]
        public string? Produto { get; set; }
    }
}
