using Microsoft.EntityFrameworkCore;
using BancoApi;
namespace BancoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<Wallet> Wallets { get; set; }
    
    public DbSet<User> Users { get; set; }
}