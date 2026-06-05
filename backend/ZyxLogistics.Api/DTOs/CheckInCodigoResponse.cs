using System.Text.Json.Serialization;

namespace ZyxLogistics.Api.DTOs
{
    public class CheckInCodigoResponse
    {
        public int AgendamentoId { get; set; }
        public string MotoristaNome { get; set; } = string.Empty;
        public string TelefoneMascarado { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public string? CodigoDesenvolvimento { get; set; }

        [JsonIgnore]
        public string Telefone { get; set; } = string.Empty;

        [JsonIgnore]
        public string Mensagem { get; set; } = string.Empty;
    }
}
