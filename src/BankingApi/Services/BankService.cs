using AutoMapper;
using BankingApi.Data.Repositories;
using BankingApi.DTOs;
using BankingApi.Models;
using BankingApi.Exceptions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Services;

public class BankService{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IWithdrawalRepository _withdrawalRepository;
    private readonly IMapper _mapper;   

    public BankService(
        IBankAccountRepository bankAccountRepository,
        IWithdrawalRepository withdrawalRepository,
        IMapper mapper
    ){
        _bankAccountRepository = bankAccountRepository;
        _withdrawalRepository = withdrawalRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BankAccountDto>> GetAccountsByHolderIdAsync(int accountHolderId){
        var accounts = await _bankAccountRepository.GetAccountsByHolderIdAsync(accountHolderId);
        return _mapper.Map<IEnumerable<BankAccountDto>>(accounts);
    }

    public async Task<WithdrawalResponseDto> CreateWithdrawalAsync(string accountNumber, WithdrawalRequestDto withdrawalRequest){
        var account = await _bankAccountRepository.GetByAccountNumberAsync(accountNumber);
        if(account == null)
            throw new NotFoundException($"Account with account number {accountNumber} not found");
        
        //Validate withdrawal
        ValidateWithdrawal(account, withdrawalRequest.Amount);

        //Create withdrawal record
        var withdrawal = new Withdrawal{
            Amount = withdrawalRequest.Amount,
            TransactionDate = DateTime.UtcNow,
            TransactionReference = GenerateTransactionReference(),
            BankAccountId = account.Id,

        };

        //Update account balance
        account.AvailableBalance -= withdrawalRequest.Amount;

        //Save changes
        await _withdrawalRepository.AddAsync(withdrawal);
        await _bankAccountRepository.UpdateAsync(account);

        return new WithdrawalResponseDto{
            TransactionReference = withdrawal.TransactionReference,
            Amount = withdrawal.Amount,
            TransactionDate = withdrawal.TransactionDate,
            NewBalance = account.AvailableBalance
        };
         
       
    }

    private void ValidateWithdrawal(BankAccount account, decimal amount){
        if (amount <= 0)
        throw new ValidationException("Withdrawal amount must be greater than zero");

        if (amount > account.AvailableBalance)
        throw new ValidationException("Withdrawal amount cannot excees available balance");

        if (account.Status == AccountStatus.Inactive)
        throw new ValidationException("Withdrawals are not allowed on inactive accounts");

        if (account.AccountType == AccountType.FixedDeposit && amount != account.AvailableBalance)
        throw new ValidationException("Fixed deposit accounts only allow 100% withdrawals");
    }

    private string GenerateTransactionReference(){
        return $"TRX-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }    
}