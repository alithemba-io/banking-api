using Microsoft.EntityFrameworkCore;
using BankingApi.Models;
using System.Text.Json;

namespace BankingApi.Data;  

public class BankDbContent : DbContext{
    public BankDbContent(DbContextOptions<BankDbContent> options) : base(options){}

    public DbSet<AccountHolder> AccountHolders {get; set; } = null!;
    public DbSet<AuditLog> AuditLogs {get; set; } = null!;
    public DbSet<BankAccount> BankAccounts {get; set; } = null!;
}