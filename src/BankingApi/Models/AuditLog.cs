using System.Runtime.CompilerServices;
using System.Text.Json; 

namespace BankingApi.Models;

public class AuditLog{
    public int Id{get; set; }
    public string EntityName{get; set; } = string.Empty;
    public string Action{get; set; } = string.Empty;
    public string EntityId{get; set; } = string.Empty;
    public string? OldValues {get; set; }
    public string? NewValues {get; set; }
    public DateTime TimeStamp{get; set; }
    public string UserId{get; set; } = string.Empty;
}