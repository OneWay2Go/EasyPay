using EasyPay.Application.DTOs.Order;
using EasyPay.Infrastructure.Database;

namespace EasyPay.Application.Interfaces;

public interface IOrderService
{
    Task<List<OrderGetDto>> GetAllAsync();
    Task<OrderGetDto> GetByIdAsync(Guid orderId);
    Task<bool> AddAsync(OrderAddDto dto);
    Task<bool> RemoveAsync(Guid orderId);
}