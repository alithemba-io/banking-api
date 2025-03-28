using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IBankService _bankService;
    
    public AccountsController(IBankService bankService)
    {
        _bankService = bankService;
    }
    
    [HttpGet("{accountHolderId}")]
    [ProducesResponseType(typeof(IEnumerable<BankAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountsByHolderId(int accountHolderId)
    {
        var accounts = await _bankService.GetAccountsByHolderIdAsync(accountHolderId);
        return Ok(accounts);
    }
    
    [HttpGet("account/{accountNumber}")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountByNumber(string accountNumber)
    {
        try
        {
            var account = await _bankService.GetAccountByNumberAsync(accountNumber);
            return Ok(account);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
    
    [HttpPost("{accountNumber}/withdraw")]
    [ProducesResponseType(typeof(WithdrawalResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(string accountNumber, [FromBody] WithdrawalRequestDto request)
    {
        try
        {
            var result = await _bankService.CreateWithdrawalAsync(accountNumber, request);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}