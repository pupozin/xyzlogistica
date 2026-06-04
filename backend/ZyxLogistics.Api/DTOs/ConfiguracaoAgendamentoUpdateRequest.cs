using System.ComponentModel.DataAnnotations;

namespace ZyxLogistics.Api.DTOs
{
    public class ConfiguracaoAgendamentoUpdateRequest
    {
        [Range(1, 1440)]
        public int IntervaloMinutos { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFim { get; set; }
    }
}
