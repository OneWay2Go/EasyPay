using EasyPay.Core.Enums;

namespace EasyPay.Application.DTOs.Order;

public class OrderGetDto
{
    public Guid OrderId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    
    public string ProductTitleSnapshot { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public int Quantity { get; set; }
    
    public decimal TotalAmount => Quantity * PriceAtPurchase;
    
    public OrderStatus Status { get; set; }
}