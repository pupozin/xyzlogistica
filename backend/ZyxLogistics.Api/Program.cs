using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<IConfiguracaoAgendamentoRepository, ConfiguracaoAgendamentoRepository>();
builder.Services.AddScoped<IMotoristaRepository, MotoristaRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ITransportadoraRepository, TransportadoraRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
