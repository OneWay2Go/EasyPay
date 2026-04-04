using System.ComponentModel.DataAnnotations;

namespace TgRobuxBot.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty; // "Robux", "TelegramStars", "TelegramPremium"
    public string Name { get; set; } = string.Empty; // e.g. "400 Robux"
    public decimal PriceUZS { get; set; } 
}
