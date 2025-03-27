using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data.Repositories;

public class WithdrawalRepository : Repository<Withdrawal>, IWithdrawalRepository{
    public WithdrawalRepository (BankDbContext dbContext) : base(dbContext){}
    public async Task<IEnumerable<Withdrawal>> GetWithdrawalsByAccountIdAsync(int bankAccountId){
        return await _dbContext.Withdrawals
        .Where(w => w.BankAccountId == bankAccountId)
        .ToListAsync();
    }
}