namespace EasyPay.Application.DTOs.Product;

public class ProductAddDto
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public decimal CurrentPrice { get; set; }
    
    public int StockQuantity { get; set; }
    
    public Guid SellerId { get; set; }
    
    public Guid GameId  { get; set; }
}