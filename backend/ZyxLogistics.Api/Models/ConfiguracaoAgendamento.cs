namespace ZyxLogistics.Api.Models
{
    public class ConfiguracaoAgendamento
    {
        public int Id { get; set; }
        public int IntervaloMinutos { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
