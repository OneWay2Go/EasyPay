using System.ComponentModel.DataAnnotations;

namespace TgRobuxBot.Models;

public class AppUser
{
    [Key]
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string State { get; set; } = "Idle";
    
    // Additional state info like ProductId being bought
    public int? PendingProductId { get; set; }
    // Temporary data stored during the order pipeline (JSON string)
    public string? TempData { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
