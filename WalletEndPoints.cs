using BancoApi.Data;
using Microsoft.EntityFrameworkCore;
using BancoApi.Services;
using BancoApi.DTOs;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BancoApi;

public static class WalletEndPoints
{
    public static void MapWalletEndpoints(this WebApplication app)
    {
        app.MapPost("/criar-conta", async (WalletServices service, CriarContaRequest request) =>
        {
            var novaCarteira = await service.CriarConta(request);
            var response = new ContaResponse(novaCarteira.Id, novaCarteira.Owner, novaCarteira.Balance);
            return Results.Ok(response);
            
        });
        app.MapGet("/listar-contas", async (WalletServices service) =>
        {
            var todasContas = await service.ListarContas();
            var response = todasContas.Select(x => new ContaResponse(x.Id, x.Owner, x.Balance));
            return Results.Ok(response);
        });

        app.MapGet("/saldo/{id}", async (WalletServices service, Guid id) =>
        {
            var carteira = await service.ObterSaldo(id);
            if (carteira == null)
            {
                return Results.NotFound("Carteira nao encontrada.");
            }
            return Results.Ok(new ContaResponse(carteira.Id, carteira.Owner, carteira.Balance));
        });
        app.MapPost("/deposit", async (WalletServices service, DepositoRequest request) =>
        {
                var carteira = await service.Depositar(request);
                if (carteira == null)
                {
                    return Results.NotFound("Carteira nao encontrada.");
                }
                return Results.Ok(new ContaResponse(carteira.Id, carteira.Owner, carteira.Balance));
            

            
        });
        app.MapGet("/extrato/{id}", async (WalletServices service, Guid id) =>
        {
            var carteira = await service.ObterSaldo(id);
            if (carteira == null){return Results.NotFound("Carteira nao encontrada.");}

            return Results.Ok(new ExtratoResponse(carteira.Owner, carteira.Balance, carteira.History));
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