namespace BankingApi.Models;

public class Withdrawal{
    public int Id{get; set; }
    public decimal Amount{get; set; }
    public DateTime TransactionDate{get; set; }
    public string TransactionReference {get; set; } = string.Empty;

    public int BankAccountId{get; set; }
    public BankAccount? BankAccount{get; set; }

}