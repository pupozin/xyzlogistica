using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class MotoristaFilterRequest
    {
        [StringLength(150)]
        public string? Nome { get; set; }

        [StringLength(30)]
        public string? Cnh { get; set; }
    }
}
