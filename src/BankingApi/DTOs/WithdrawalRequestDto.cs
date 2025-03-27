using System.ComponentModel.DataAnnotations;

namespace BankingApi.DTOs;

public class WithdrawalRequestDto{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage =  "Amount must be greater than 0")]
    public decimal Amount { get; set; }

}