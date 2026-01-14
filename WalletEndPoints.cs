namespace BancoApi;

public static class WalletEndPoints
{
    public static void MapWalletEndpoints(this WebApplication app, List<Wallet> bancoDeDados)
    {
        app.MapGet("/criar-conta", (string nome) =>
        {
            var novaCarteira = new Wallet(nome);
            bancoDeDados.Add(novaCarteira);
            return Results.Ok(new
            {
                Mensagem = "Conta criada com sucesso!",
                Id = novaCarteira.Id,
                Dono = novaCarteira.Owner
            });

        });
        app.MapGet("/listar-contas", () =>
        {
            return bancoDeDados;
        });

        app.MapGet("/saldo", (Guid id) =>
        {
            var carteira = bancoDeDados.FirstOrDefault(x => x.Id == id);
            if (carteira == null)
            {
                return Results.NotFound("Carteira nao encontrada.");
            }

            return Results.Ok(new { Dono = carteira.Owner, Saldo = carteira.Balance });
        });
        app.MapPost("/deposit", async (PedidoDeposito request) =>
        {
            var carteira = bancoDeDados.FirstOrDefault(x => x.Id == request.Id);
            if (carteira == null){return Results.NotFound("Carteira nao encontrada.");}

            await carteira.DepositAsync(request.Amount);
            return Results.Ok(carteira.Balance);
        });
        app.MapGet("/extrato", (Guid id) =>
        {
            var carteira = bancoDeDados.FirstOrDefault(x => x.Id == id);
            if (carteira == null){return Results.NotFound("Carteira nao encontrada.");}

            return Results.Ok(new
                { Dono = carteira.Owner, Historico = carteira.History }
            );
        });
        app.MapGet("/transfer", (Guid id, Guid toId, decimal amount ) => 
        {
            var carteira1 = bancoDeDados.FirstOrDefault(x => x.Id == id);
            if (carteira1 == null)
            {
                return Results.NotFound("Carteira de origem nao encontrada.");
            }
            var carteira2 = bancoDeDados.FirstOrDefault(x => x.Id == toId);
            if (carteira2 == null)
            {
                return Results.NotFound("Carteira de destino nao encontrada.");
            }
            bool result = carteira1.Transfer(carteira2, amount);
            if (result)
            {
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