namespace EasyPay.Application.DTOs.Order;

public class OrderAddDto
{
    public Guid ProductId { get; set; }
    
    public Guid BuyerId { get; set; }
    
    public Guid SellerId { get; set; }
    
    public string ProductTitleSnapshot { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public int Quantity { get; set; }
}