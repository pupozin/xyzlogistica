namespace ZyxLogistics.Api.Models
{
    public class OperacaoAba
    {
        public int StatusId { get; set; }
        public string StatusDescricao { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public IReadOnlyList<Agendamento> Agendamentos { get; set; } = Array.Empty<Agendamento>();
    }
}
