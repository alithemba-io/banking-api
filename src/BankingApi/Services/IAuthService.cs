namespace BankingApi.Services;

public interface IAuthService{
    string GenerateToken(string username);  
}