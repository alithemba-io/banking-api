using BankingApi.DTOs;
using BankingApi.Models;

namespace BankingApi.Services;

public interface IBankService{
    Task<IEnumerable<BankAccountDto>> GetAccountsByHolderIdAsync(int accountHolderId);
    Task<BankAccountDto> GetAccountByAccountNumberAsync(string accountNumber);
    Task<WithdrawalResponseDto> CreateWithdrawalAsync(string accountNumber, WithdrawalRequestDto withdrawalRequest);
}