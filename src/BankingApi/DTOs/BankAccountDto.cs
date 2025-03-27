namespace BankingApi.DTOs;

public class BankAccountDto{
    public string AccountNumber { get; set; }= string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;  
    public decimal AvailableBalance { get; set; }


}
