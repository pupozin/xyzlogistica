using System.Net.Http.Headers;
using System.Text;

namespace ZyxLogistics.Api.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TwilioSmsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task EnviarAsync(string telefone, string mensagem)
        {
            var accountSid = _configuration["Sms:Twilio:AccountSid"];
            var authToken = _configuration["Sms:Twilio:AuthToken"];
            var from = _configuration["Sms:Twilio:From"];

            if (string.IsNullOrWhiteSpace(accountSid) ||
                string.IsNullOrWhiteSpace(authToken) ||
                string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException("Configuracao da Twilio incompleta.");
            }

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["From"] = from,
                ["To"] = NormalizarTelefone(telefone),
                ["Body"] = mensagem
            });

            var response = await _httpClient.PostAsync(
                $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha ao enviar SMS pela Twilio: {body}");
            }
        }

        private static string NormalizarTelefone(string telefone)
        {
            var valor = telefone.Trim();

            if (valor.StartsWith("+", StringComparison.Ordinal))
            {
                return valor;
            }

            var apenasDigitos = new string(valor.Where(char.IsDigit).ToArray());

            if (apenasDigitos.StartsWith("55", StringComparison.Ordinal))
            {
                return $"+{apenasDigitos}";
            }

            return $"+55{apenasDigitos}";
        }
    }
}
