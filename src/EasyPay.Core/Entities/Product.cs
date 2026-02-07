namespace EasyPay.Core.Entities;

public class Product
{
    public Guid ProductId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public decimal CurrentPrice { get; set; }
    
    public int StockQuantity { get; set; }
    
    public bool IsDeleted { get; set; }
    
    // Relationships
    public Guid SellerId { get; set; }
    public User Seller { get; set; }
    
    public Guid GameId { get; set; }
    public Game Game { get; set; }
}