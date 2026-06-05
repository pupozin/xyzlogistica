using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class AgendamentoFilterRequest
    {
        [Required]
        public DateTime Data { get; set; }
    }
}
