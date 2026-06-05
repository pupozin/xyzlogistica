namespace ZyxLogistics.Api.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int OperacaoId { get; set; }
        public string OperacaoDescricao { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusDescricao { get; set; } = string.Empty;
        public int VeiculoId { get; set; }
        public string VeiculoPlaca { get; set; } = string.Empty;
        public string TipoVeiculo { get; set; } = string.Empty;
        public int TransportadoraId { get; set; }
        public string TransportadoraNome { get; set; } = string.Empty;
        public int? LocalId { get; set; }
        public string? LocalDescricao { get; set; }
        public int MotoristaId { get; set; }
        public string MotoristaNome { get; set; } = string.Empty;
        public string MotoristaCnh { get; set; } = string.Empty;
        public DateTime DataHoraAgendada { get; set; }
        public DateTime? DataHoraChegada { get; set; }
        public DateTime? DataHoraDoca { get; set; }
        public DateTime? DataHoraFinalizado { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
