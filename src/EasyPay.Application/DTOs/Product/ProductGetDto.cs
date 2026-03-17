namespace EasyPay.Application.DTOs.Product;

public class ProductGetDto
{
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public decimal CurrentPrice { get; set; }
    
    public int StockQuantity { get; set; }
}