using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Models;

public class AuditEntry{
    public EntityEntry Entry {get; } 
    public string EntityName {get; set; } = string.Empty;
    public string Action {get; set; } = string.Empty;
    public string EntityId {get; set; } = string.Empty;
    public Dictionary<string, object> OldValues {get; } = new();
    public Dictionary<string, object> NewValues {get; } = new();
    public string UserId {get; set; } = string.Empty;
    public DateTime TimeStamp {get; set; } = DateTime.UtcNow;

    public AuditEntry(EntityEntry entry){
        Entry = entry;
        EntityName = entry.Entity.GetType().Name;        
    }

    public AuditLog ToAuditLog(){
        return new AuditLog{
            EntityName = EntityName,
            Action = Action,
            EntityId = EntityId,
            OldValues = OldValues.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(OldValues) : null,
            NewValues = NewValues.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(NewValues) : null,
            TimeStamp = TimeStamp,
            UserId = UserId
        };
    }
}