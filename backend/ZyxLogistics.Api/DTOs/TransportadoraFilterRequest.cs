using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class TransportadoraFilterRequest
    {
        [StringLength(150)]
        public string? Nome { get; set; }

        [StringLength(20)]
        public string? Cnpj { get; set; }
    }
}
