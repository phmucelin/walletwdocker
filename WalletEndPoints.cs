using BancoApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BancoApi;

public static class WalletEndPoints
{
    public static void MapWalletEndpoints(this WebApplication app)
    {
        app.MapPost("/criar-conta", async (AppDbContext db, string nome) =>
        {
            var novaCarteira = new Wallet(nome);
            db.Wallets.Add(novaCarteira);
            await db.SaveChangesAsync();
            return Results.Ok(new
            {
                Mensagem = "Conta criada com sucesso!",
                Id = novaCarteira.Id,
                Dono = novaCarteira.Owner
            });

        });
        app.MapGet("/listar-contas", async (AppDbContext db) =>
        {
            var todasContas = await db.Wallets.ToListAsync();
            return Results.Ok(todasContas);
        });

        app.MapGet("/saldo/{id}", async (AppDbContext db, Guid id) =>
        {
            var carteira = await db.Wallets.FindAsync(id);
            if (carteira == null)
            {
                return Results.NotFound("Carteira nao encontrada.");
            }

            return Results.Ok(new { Dono = carteira.Owner, Saldo = carteira.Balance });
        });
        app.MapPost("/deposit", async (AppDbContext db, PedidoDeposito request) =>
        {
            var carteira = await db.Wallets.FindAsync(request.Id);
            if (carteira == null){return Results.NotFound("Carteira nao encontrada.");}
            await carteira.DepositAsync(request.Amount);
            await db.SaveChangesAsync();
            return Results.Ok(carteira.Balance);
        });
        app.MapGet("/extrato/{id}", async (AppDbContext db, Guid id) =>
        {
            var carteira = await db.Wallets.FindAsync(id);
            if (carteira == null){return Results.NotFound("Carteira nao encontrada.");}

            return Results.Ok(new
                { Dono = carteira.Owner, Historico = carteira.History }
            );
        });
        app.MapPost("/transfer", async (AppDbContext db, PedidoTransferencia request ) => 
        {
            var carteira1 = await db.Wallets.FindAsync(request.IdOrigem);
            if (carteira1 == null)
            {
                return Results.NotFound("Carteira de origem nao encontrada.");
            }
            var carteira2 = await db.Wallets.FindAsync(request.IdDestino);
            if (carteira2 == null)
            {
                return Results.NotFound("Carteira de destino nao encontrada.");
            }
            bool result = carteira1.Transfer(carteira2, request.Amount);
            if (result)
            {
                await db.SaveChangesAsync();
                return Results.Ok(new
                {
                    Mensagem = "Transfer com sucesso!",
                    SaldoOrigem = carteira1.Balance,
                    SaldoDestino = carteira2.Balance
                });
            }
            else
            {
                return Results.BadRequest("Saldo insuficiente.");
            }
        });
    }
}