using AutoMapper;
using BankingApi.Data.Repositories;
using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Models;
using BankingApi.Services;
using Moq;
using Xunit;

namespace BankingApi.Tests.Services;

public class BankServiceTests
{
    private readonly Mock<IBankAccountRepository> _mockBankAccountRepository;
    private readonly Mock<IWithdrawalRepository> _mockWithdrawalRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly BankService _bankService;
    
    public BankServiceTests()
    {
        _mockBankAccountRepository = new Mock<IBankAccountRepository>();
        _mockWithdrawalRepository = new Mock<IWithdrawalRepository>();
        _mockMapper = new Mock<IMapper>();
        
        _bankService = new BankService(
            _mockBankAccountRepository.Object,
            _mockWithdrawalRepository.Object,
            _mockMapper.Object
        );
    }
    
    [Fact]
    public async Task GetAccountByNumberAsync_ShouldReturnAccount_WhenAccountExists()
    {
        // Arrange
        var accountNumber = "123";
        var account = new BankAccount { AccountNumber = accountNumber };
        var accountDto = new BankAccountDto { AccountNumber = accountNumber };
        
        _mockBankAccountRepository.Setup(r => r.GetByAccountNumberAsync(accountNumber))
            .ReturnsAsync(account);
        _mockMapper.Setup(m => m.Map<BankAccountDto>(account))
            .Returns(accountDto);
            
        // Act
        var result = await _bankService.GetAccountByNumberAsync(accountNumber);
        
        // Assert
        Assert.Equal(accountDto, result);
    }
    
    [Fact]
    public async Task GetAccountByNumberAsync_ShouldThrowNotFoundException_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountNumber = "123";
        
        _mockBankAccountRepository.Setup(r => r.GetByAccountNumberAsync(accountNumber))
            .ReturnsAsync((BankAccount?)null);
            
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _bankService.GetAccountByNumberAsync(accountNumber));
    }
    
    [Fact]
    public async Task CreateWithdrawalAsync_ShouldThrowValidationException_WhenAmountIsZero()
    {
        // Arrange
        var accountNumber = "123";
        var account = new BankAccount
        {
            Id = 1,
            AccountNumber = accountNumber,
            Status = AccountStatus.Active,
            AvailableBalance = 1000
        };
        
        _mockBankAccountRepository.Setup(r => r.GetByAccountNumberAsync(accountNumber))
            .ReturnsAsync(account);
            
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _bankService.CreateWithdrawalAsync(accountNumber, new WithdrawalRequestDto { Amount = 0 }));
    }
    
    [Fact]
    public async Task CreateWithdrawalAsync_ShouldThrowValidationException_WhenAmountExceedsBalance()
    {
        // Arrange
        var accountNumber = "123";
        var account = new BankAccount
        {
            Id = 1,
            AccountNumber = accountNumber,
            Status = AccountStatus.Active,
            AvailableBalance = 1000
        };
        
        _mockBankAccountRepository.Setup(r => r.GetByAccountNumberAsync(accountNumber))
            .ReturnsAsync(account);
            
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _bankService.CreateWithdrawalAsync(accountNumber, new WithdrawalRequestDto { Amount = 1500 }));
    }
    
    [Fact]
    public async Task CreateWithdrawalAsync_ShouldThrowValidationException_WhenAccountIsInactive()
    {
        // Arrange
        var accountNumber = "123";
        var account = new BankAccount
        {
            Id = 1,
            AccountNumber = accountNumber,
            Status = AccountStatus.Inactive,
            AvailableBalance = 1000
        };
        
        _mockBankAccountRepository.Setup(r => r.GetByAccountNumberAsync(accountNumber))
            .ReturnsAsync(account);
            
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _bankService.CreateWithdrawalAsync(accountNumber, new WithdrawalRequestDto { Amount = 500 }));
    }
    
    [Fact]
    public async Task CreateWithdrawalAsync_ShouldThrowValidationException_WhenFixedDepositWithPartialWithdrawal()
    {
        // Arrange
        var accountNumber = "123";
        var account = new BankAccount
        {
            Id = 1,
            AccountNumber = accountNumber,
            AccountType = AccountType.FixedDeposit,
            Status = AccountStatus.Active,
            AvailableBalance = 1000
        };
        
        _mockBankAccountRepository.Setup(r => r.GetByAccountNumberAsync(accountNumber))
            .ReturnsAsync(account);
            
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _bankService.CreateWithdrawalAsync(accountNumber, new WithdrawalRequestDto { Amount = 500 }));
    }
    
    [Fact]
    public async Task CreateWithdrawalAsync_ShouldSucceed_WhenAllValidationsPass()
    {
        // Arrange
        var accountNumber = "123";
        var account = new BankAccount
        {
            Id = 1,
            AccountNumber = accountNumber,
            AccountType = AccountType.Cheque,
            Status = AccountStatus.Active,
            AvailableBalance = 1000
        };
        
        _mockBankAccountRepository.Setup(r => r.GetByAccountNumberAsync(accountNumber))
            .ReturnsAsync(account);
            
        // Act
        var result = await _bankService.CreateWithdrawalAsync(accountNumber, new WithdrawalRequestDto { Amount = 500 });
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.Amount);
        Assert.Equal(500, result.NewBalance);
        
        _mockWithdrawalRepository.Verify(r => r.AddAsync(It.IsAny<Withdrawal>()), Times.Once);
        _mockBankAccountRepository.Verify(r => r.UpdateAsync(It.Is<BankAccount>(a => a.AvailableBalance == 500)), Times.Once);
    }
}