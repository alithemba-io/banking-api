using Microsoft.EntityFrameworkCore;
using BankingApi.Models;
using System.Text.Json;

namespace BankingApi.Data;  

public class BankDbContext : DbContext{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options){}

    public DbSet<AccountHolder> AccountHolders {get; set; } = null!;
    public DbSet<AuditLog> AuditLogs {get; set; } = null!;
    public DbSet<Withdrawal> Withdrawals {get; set; } = null!;
    public DbSet<BankAccount> BankAccounts {get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder){
    base.OnModelCreating(modelBuilder);
    
    // Configure relationships
    modelBuilder.Entity<BankAccount>()
        .HasOne(b => b.AccountHolder)
        .WithMany(a => a.BankAccounts)
        .HasForeignKey(b => b.AccountHolderId);
        
    modelBuilder.Entity<Withdrawal>()
        .HasOne(w => w.BankAccount)
        .WithMany(b => b.Withdrawals)
        .HasForeignKey(w => w.BankAccountId);
    
    // Configure decimal precision
    modelBuilder.Entity<BankAccount>()
        .Property(b => b.AvailableBalance)
        .HasPrecision(18, 2);
        
    modelBuilder.Entity<Withdrawal>()
        .Property(w => w.Amount)
        .HasPrecision(18, 2);
    }  

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default){
        var auditEntries = OnBeforeSaveChanges(); // Track changes before they are committed

        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChangesAsync(auditEntries, cancellationToken); 

        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges(){
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries()){
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry){
                Action = entry.State.ToString(),
                EntityId = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue?.ToString() ?? string.Empty,
                UserId = "system"
            };

            foreach (var property in entry.Properties){
                if (property.Metadata.IsPrimaryKey() || property.Metadata.IsForeignKey())
                continue;

                string propertyName = property.Metadata.Name;

        
                if (entry.State == EntityState.Added){
                    auditEntry.NewValues[propertyName] = property.CurrentValue ?? string.Empty;
                }
                else if (entry.State == EntityState.Deleted){
                    auditEntry.OldValues[propertyName] = property.OriginalValue ?? string.Empty;

                }
                else if (entry.State == EntityState.Modified && property.IsModified){
                    auditEntry.OldValues[propertyName] = property.OriginalValue ?? string.Empty;
                    auditEntry.NewValues[propertyName] = property.CurrentValue ?? string.Empty;
                }    

            }
            auditEntries.Add(auditEntry);
        }
        return auditEntries;    
    }

    private async Task OnAfterSaveChangesAsync(List<AuditEntry> auditEntries, CancellationToken cancellationToken = default){
        if (auditEntries == null || auditEntries.Count == 0)
        return;

        //Create Audit Logs from the Audit Entries
        foreach (var auditEntry in auditEntries){ 
            var auditLog = auditEntry.ToAuditLog();
            AuditLogs.Add(auditLog);
        }
        await SaveChangesAsync(cancellationToken); //Save audit Logs to the database
    }
}

