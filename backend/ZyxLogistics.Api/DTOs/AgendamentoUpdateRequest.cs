using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class AgendamentoUpdateRequest
    {
        [Range(1, int.MaxValue)]
        public int OperacaoId { get; set; }

        [Range(1, int.MaxValue)]
        public int TransportadoraId { get; set; }

        [Range(1, int.MaxValue)]
        public int VeiculoId { get; set; }

        [Range(1, int.MaxValue)]
        public int MotoristaId { get; set; }

        public DateTime DataHoraAgendada { get; set; }
    }
}
