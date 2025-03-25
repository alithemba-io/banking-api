namespace BankingApi.Models;

public class BankAccount{
    public int Id{get; set; }
    public string AccountNumber{get; set; } = string.Empty;
    public AccountType AccountType{get; set; }
    public string Name{get; set; } = string.Empty;
    public AccountStatus Status{get; set; }
    public decimal AvailableBalance {get; set; }

    public int AccountHolderId{get; set; }
    public AccountHolder? AccountHolder{get; set; }

    public ICollection<Withdrawal> Withdrawals{get; set; } = new List<Withdrawal>();


}