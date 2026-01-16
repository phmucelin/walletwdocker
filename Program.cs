using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BancoApi;
using BancoApi.Data;
using Microsoft.EntityFrameworkCore;
using BancoApi.Services; // Importante para achar o WalletServices

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Middleware de Erros (O Porteiro)
builder.Services.AddExceptionHandler<BancoApi.Middlewares.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 3. Registro do Service 
builder.Services.AddScoped<WalletServices>();

// 4. Swagger
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();           

var app = builder.Build();

// 5. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI(); 
}

// Ativa o Middleware de Erro
app.UseExceptionHandler(); 

app.UseHttpsRedirection();

// Seus Endpoints
app.MapWalletEndpoints();

app.Run();