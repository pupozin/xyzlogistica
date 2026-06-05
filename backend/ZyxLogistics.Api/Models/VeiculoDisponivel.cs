namespace ZyxLogistics.Api.Models
{
    public class VeiculoDisponivel
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string TipoVeiculo { get; set; } = string.Empty;
        public int TransportadoraId { get; set; }
        public string TransportadoraNome { get; set; } = string.Empty;
    }
}
