namespace TgRobuxBot.Services;

public class OrderSessionData
{
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    
    // For Custom Stars Amount
    public decimal CustomPriceUZS { get; set; }

    // Roblox Data
    public string? RobloxUsername { get; set; }
    public string? RobloxPasswordEncrypted { get; set; }
    public string? Roblox2FA { get; set; } // "Yes (Contact Admin)" or "No"

    // Telegram Data
    public string? TargetTelegramUsername { get; set; }
    public string? PhoneNumber { get; set; }

    // General
    public string? Category { get; set; }
}
