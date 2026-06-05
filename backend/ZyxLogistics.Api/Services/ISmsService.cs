namespace ZyxLogistics.Api.Services
{
    public interface ISmsService
    {
        Task EnviarAsync(string telefone, string mensagem);
    }
}
