using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class VeiculoFilterRequest
    {
        [StringLength(10)]
        public string? Placa { get; set; }

        [StringLength(80)]
        public string? TipoVeiculo { get; set; }

        public int? TransportadoraId { get; set; }
    }
}
