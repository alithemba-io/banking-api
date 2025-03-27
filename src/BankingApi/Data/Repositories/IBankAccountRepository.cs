using BankingApi.Models;

namespace BankingApi.Data.Repositories;

public interface IBankAccountRepository : IRepository<BankAccount>{
    Task<IEnumerable<BankAccount>> GetAccountsByHolderIdAsync(int accountHolderId);
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);
}
