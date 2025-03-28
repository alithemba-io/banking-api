using BankingApi.Controllers;
using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BankingApi.Tests.Controllers;

public class AccountsControllerTests
{
    private readonly Mock<IBankService> _mockBankService;
    private readonly AccountsController _controller;
    
    public AccountsControllerTests()
    {
        _mockBankService = new Mock<IBankService>();
        _controller = new AccountsController(_mockBankService.Object);
    }
    
    [Fact]
    public async Task GetAccountByNumber_ShouldReturnOk_WhenAccountExists()
    {
        // Arrange
        var accountNumber = "123";
        var accountDto = new BankAccountDto { AccountNumber = accountNumber };
        
        _mockBankService.Setup(s => s.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync(accountDto);
            
        // Act
        var result = await _controller.GetAccountByNumber(accountNumber);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedAccount = Assert.IsType<BankAccountDto>(okResult.Value);
        Assert.Equal(accountDto, returnedAccount);
    }
    
    [Fact]
    public async Task GetAccountByNumber_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountNumber = "123";
        
        _mockBankService.Setup(s => s.GetAccountByNumberAsync(accountNumber))
            .ThrowsAsync(new NotFoundException($"Account with number {accountNumber} not found"));
            
        // Act
        var result = await _controller.GetAccountByNumber(accountNumber);
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async Task Withdraw_ShouldReturnOk_WhenWithdrawalIsSuccessful()
    {
        // Arrange
        var accountNumber = "123";
        var request = new WithdrawalRequestDto { Amount = 500 };
        var response = new WithdrawalResponseDto
        {
            TransactionReference = "TRX-12345678",
            Amount = 500,
            TransactionDate = DateTime.UtcNow,
            NewBalance = 500
        };
        
        _mockBankService.Setup(s => s.CreateWithdrawalAsync(accountNumber, request))
            .ReturnsAsync(response);
            
        // Act
        var result = await _controller.Withdraw(accountNumber, request);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<WithdrawalResponseDto>(okResult.Value);
        Assert.Equal(response, returnedResponse);
    }
    
    [Fact]
    public async Task Withdraw_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var accountNumber = "123";
        var request = new WithdrawalRequestDto { Amount = 0 };
        
        _mockBankService.Setup(s => s.CreateWithdrawalAsync(accountNumber, request))
            .ThrowsAsync(new ValidationException("Withdrawal amount must be greater than zero"));
            
        // Act
        var result = await _controller.Withdraw(accountNumber, request);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}