using BankingApi.Models;
using Microsoft.EntityFrameworkCore; 
using BankingApi.Data;
using BankingApi.Data.Repositories;

namespace BankingApi.Data.Repositories;

public class BankAccountRepository : Repository<BankAccount>, IBankAccountRepository{
    public BankAccountRepository(BankDbContext dbContext) : base(dbContext){}
    public async Task<IEnumerable<BankAccount>> GetAccountsByHolderIdAsync(int accountHolderId){
        return await _dbContext.BankAccounts
            .Where(b => b.AccountHolderId == accountHolderId)
            .Include(b => b.AccountHolder)
            .ToListAsync();
    }

    public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber){
        return await _dbContext.BankAccounts
            .Include(b => b.AccountHolder)
            .FirstOrDefaultAsync(b => b.AccountNumber == accountNumber);        
    }
}