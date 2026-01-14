using BancoApi;
using BancoApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoApi.Services;

public class WalletServices
{
    private readonly AppDbContext _db;
    
    public WalletServices(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Wallet> CriarConta(PedidoCriacao request)
    {
        var novaCarteira = new Wallet(request.Nome);
        _db.Wallets.Add(novaCarteira);
        await _db.SaveChangesAsync();
        return novaCarteira;
    }

    public async Task<List<Wallet>> ListarContas()
    {
        return await _db.Wallets.ToListAsync();
    }

    public async Task<Wallet?> ObterSaldo(Guid id)
    {
        return await _db.Wallets.FindAsync(id);
    }

    public async Task<Wallet?> Depositar(PedidoDeposito request)
    {
        var conta = await _db.Wallets.FindAsync(request.Id);
        if (conta == null)
        {
            return null;
        }

        var sucesso = await conta.DepositAsync(request.Amount);
        if (!sucesso)
        {
            throw new ArgumentException("Voce nao pode depositar valores menores ou iguais a zero.");
        }
        await  _db.SaveChangesAsync();
        return conta;
    }

    public async Task<TransferenciaResult> Transferir(PedidoTransferencia request)
    {
        var origem = await _db.Wallets.FindAsync(request.IdOrigem);
        var destino = await _db.Wallets.FindAsync(request.IdDestino);
        if (origem == null)
        {
            return new TransferenciaResult(false, "A conta de origem nao existe.");
        }
        if (destino == null)
        {
            return new TransferenciaResult(false, "A conta de destino nao existe.");
        }
        bool sucesso = origem.Transfer(destino, request.Amount);
        if (!sucesso)
        {
            return new TransferenciaResult(false, "Saldo insuficiente.");
        }
        await _db.SaveChangesAsync();
        return new TransferenciaResult(true, "Transferencia realizada com sucesso.", origem.Balance, destino.Balance);
    }
    public record TransferenciaResult(bool Sucesso, string Mensagem, decimal SaldoOrigem = 0, decimal SaldoDestino = 0);
    
}