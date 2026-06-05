namespace ZyxLogistics.Api.Services
{
    public class SimulatedSmsService : ISmsService
    {
        public Task EnviarAsync(string telefone, string mensagem)
        {
            return Task.CompletedTask;
        }
    }
}
