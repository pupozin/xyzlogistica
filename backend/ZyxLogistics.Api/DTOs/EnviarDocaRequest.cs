using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class EnviarDocaRequest
    {
        [Range(1, int.MaxValue)]
        public int LocalId { get; set; }
    }
}
