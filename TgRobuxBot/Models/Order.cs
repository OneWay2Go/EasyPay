using System.ComponentModel.DataAnnotations;

namespace TgRobuxBot.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    public long UserId { get; set; }
    public AppUser User { get; set; } = null!;
    
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string Status { get; set; } = "PendingPayment"; // PendingPayment, Paid, Approved, Rejected
    public string? ReceiptFileId { get; set; } // Telegram File ID for the receipt photo
    
    // Some metadata for the order (like Username for robux transfer)
    public string? AdditionalInfo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
