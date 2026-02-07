using EasyPay.Core.Enums;

namespace EasyPay.Core.Entities;

public class User
{
    public Guid UserId { get; set; }
    
    public string Username { get; set; }
    
    public string PasswordHash { get; set; }
    
    public string Email { get; set; }
    
    public decimal WalletBalance { get; set; } = 0.00m;
    
    public UserRole Role { get; set; } = UserRole.User;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Purchases { get; set; } = new List<Order>();
    public ICollection<Order> Sales { get; set; } = new List<Order>();
}
    