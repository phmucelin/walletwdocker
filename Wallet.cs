using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BancoApi;

public class Wallet
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Owner { get; set; }
    public decimal Balance { get; private set; }

    public List<string> History { get; private set; } = new();

    public Wallet()
    {
    }

    public Wallet(string owner)
    {
        this.Id = Guid.NewGuid();
        this.Owner = owner;
        this.Balance = 0;
        History = new List<string>();
    }

    public async Task<bool> DepositAsync(decimal amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        await Task.Delay(2000);
        this.Balance += amount;
        this.History.Add("Deposit:  " + amount.ToString("C"));
        return true;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0 || this.Balance < amount) return false;
        this.Balance -= amount;
        this.History.Add("Withdraw:  " + amount.ToString("C"));
        return true;
    }

    public bool Transfer(Wallet destination, decimal amount)
    {
        if (this.Balance < amount || amount <= 0 || this.Id == destination.Id)
        {
            return false;
        }

        this.Balance -= amount;
        this.History.Add($"Transfer to {destination.Id}:  " + amount.ToString("C"));
        destination.Balance += amount;
        destination.History.Add($"Received of {this.Id}: " + amount.ToString("C"));
        return true;
    }

}
public record PedidoTransferencia(Guid IdOrigem, Guid IdDestino, decimal Amount);