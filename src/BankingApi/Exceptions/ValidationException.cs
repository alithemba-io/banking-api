namespace BankingApi.DTOs;

public class ValidationException : Exception{
    public ValidationException(string message) : base(message){}
}