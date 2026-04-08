namespace EasyPay.Core.Entities;

public class Game
{
    public Guid GameId { get; set; }
    
    public string Name { get; set; }
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}