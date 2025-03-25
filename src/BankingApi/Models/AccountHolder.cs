namespace BankingApi.Models;

public class AccountHolder{
    public int Id {get; set;}
    public string FirstName {get; set; } = string.Empty;
    public string LastName {get; set; } = string.Empty;
    public DateTime DateOfBirth {get; set;}
    public string IdNumber {get; set; } = string.Empty;
    public string ResidentialAddress {get; set; } = string.Empty;
    public string MobileNumber {get; set; } = string.Empty;
    public string Email {get; set; } = string.Empty;

    //Navigation
    public ICollection<BankAccount> BankAccounts {get; set; } = new List<BankAccount>();

}