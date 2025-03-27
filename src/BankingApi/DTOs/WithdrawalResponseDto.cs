namespace BankingApi.DTOs;

public class WithdrawalResponseDto{

    public string TransactionReference {get; set; } = string.Empty;
    public decimal Amount {get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal NewBalance { get; set; } 
}