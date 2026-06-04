namespace ZyxLogistics.Api.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string TipoVeiculo { get; set; } = string.Empty;
        public int TransportadoraId { get; set; }
        public string TransportadoraNome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
