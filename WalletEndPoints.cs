using BancoApi.Data;
using Microsoft.EntityFrameworkCore;
using BancoApi.Services;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BancoApi;

public static class WalletEndPoints
{
    public static void MapWalletEndpoints(this WebApplication app)
    {
        app.MapPost("/criar-conta", async (WalletServices service, PedidoCriacao request) =>
        {
            try
            {
                var novaCarteira = await service.CriarConta(request);

                return Results.Ok(new
                {
                    Mensagem = "Conta criada com sucesso!",
                    Id = novaCarteira.Id,
                    Dono = novaCarteira.Owner
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
            

        });
        app.MapGet("/listar-contas", async (WalletServices service) =>
        {
            var todasContas = await service.ListarContas();
            return Results.Ok(todasContas);
        });

        app.MapGet("/saldo/{id}", async (WalletServices service, Guid id) =>
        {
            var carteira = await service.ObterSaldo(id);
            if (carteira == null)
            {
                return Results.NotFound("Carteira nao encontrada.");
            }
            return Results.Ok(new { Dono = carteira.Owner, Saldo = carteira.Balance });
        });
        app.MapPost("/deposit", async (WalletServices service, PedidoDeposito request) =>
        {
            try
            {
                var carteira = await service.Depositar(request);
                if (carteira == null)
                {
                    return Results.NotFound("Carteira nao encontrada.");
                }

                return Results.Ok(carteira.Balance);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
            
        });
        // Preciso criar essa no services.
        app.MapGet("/extrato/{id}", async (WalletServices service, Guid id) =>
        {
            var carteira = await service.ObterSaldo(id);
            if (carteira == null){return Results.NotFound("Carteira nao encontrada.");}

            return Results.Ok(new
                { Dono = carteira.Owner, Historico = carteira.History }
            );
        });
        app.MapPost("/transfer", async (WalletServices service, PedidoTransferencia request) =>
        {
            var resultado = await service.Transferir(request);
            if (resultado.Sucesso)
            {
                return Results.Ok(resultado);
            }
            else
            {
                return Results.BadRequest(resultado.Mensagem);
            }
        });
    }
}