using EasyPay.Application.DTOs.Product;

namespace EasyPay.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductGetDto>> GetAllActiveProductsAsync();
    Task<ProductGetDto> GetActiveProductByIdAsync(Guid productId);
    Task<bool> AddAsync(ProductAddDto dto);
    Task<bool> RemoveAsync(Guid productId);
}