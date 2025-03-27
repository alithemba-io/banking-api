using BankingApi.Models;

namespace BankingApi.Data.Repositories;

public interface IWithdrawalRepository : IRepository<Withdrawal>{
    Task<IEnumerable<Withdrawal>> GetWithdrawalsByAccountIdAsync(int bankAccountId);
}