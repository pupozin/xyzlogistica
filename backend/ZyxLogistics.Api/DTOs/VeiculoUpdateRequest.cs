using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class VeiculoUpdateRequest
    {
        [Required]
        [StringLength(10)]
        public string Placa { get; set; } = string.Empty;

        [Required]
        [StringLength(80)]
        public string TipoVeiculo { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int TransportadoraId { get; set; }
    }
}
