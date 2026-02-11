using EasyPay.Core.Enums;

namespace EasyPay.Core.Entities;

public class Order
{
    public Guid OrderId { get; set; }
    
    public Guid ProductId { get; set; }
    
    // Relationships
    public Guid BuyerId { get; set; }
    public User Buyer { get; set; }
    
    public Guid SellerId { get; set; }
    public User Seller { get; set; }
    
    // Snapshotted Data
    public string ProductTitleSnapshot { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public int Quantity { get; set; }
    
    public decimal TotalAmount => Quantity * PriceAtPurchase;

    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}