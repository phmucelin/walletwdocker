using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BancoApi;
// Se o Rider reclamar de "Wallet" ou "WalletEndpoints",
// clique na lâmpada amarela e dê "Import missing references".

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();           

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI(); 
}


List<Wallet> bancoDeDados = new List<Wallet>();


var c1 = new Wallet("Pedro");
bancoDeDados.Add(c1);
Console.WriteLine($"PEDRO ID: {c1.Id}"); 


app.MapWalletEndpoints(bancoDeDados);

app.Run();