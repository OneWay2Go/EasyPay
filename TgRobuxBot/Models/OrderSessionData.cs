namespace TgRobuxBot.Models;

public class OrderSessionData
{
    // Common
    public int ProductId { get; set; }
    public decimal CustomPriceUZS { get; set; } // Used for stars calculation
    public string? ProductName { get; set; }

    // Robux
    public string? RobloxUsername { get; set; }
    public string? RobloxPasswordEncrypted { get; set; }
    public string? Roblox2FA { get; set; }

    // Telegram Premium
    public string? PhoneNumber { get; set; }
    
    // Telegram Premium (Gift) & Telegram Stars
    public string? TargetTelegramUsername { get; set; }
}
