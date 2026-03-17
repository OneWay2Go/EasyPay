using EasyPay.Application.DTOs.Order;
using EasyPay.Application.Interfaces;
using EasyPay.Core.Entities;
using EasyPay.Core.Enums;
using EasyPay.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Application.Services;

public class OrderService(EPContext context) : IOrderService
{
    public async Task<List<OrderGetDto>> GetAllAsync()
    {
        var orders = await context.Orders.ToListAsync();
        List<OrderGetDto> ordersDto = new List<OrderGetDto>();
        foreach (var order in orders)
        {
            ordersDto.Add(new OrderGetDto()
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                SellerId = order.SellerId,
                BuyerId = order.BuyerId,
                PriceAtPurchase = order.PriceAtPurchase,
                Quantity = order.Quantity,
                Status = order.Status,
                ProductTitleSnapshot = order.ProductTitleSnapshot
            });
        }
        
        return ordersDto;
    }

    public async Task<OrderGetDto> GetByIdAsync(Guid orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        if (order is not null)
        {
            OrderGetDto orderDto = new OrderGetDto()
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                SellerId = order.SellerId,
                BuyerId = order.BuyerId,
                PriceAtPurchase = order.PriceAtPurchase,
                Quantity = order.Quantity,
                Status = order.Status,
                ProductTitleSnapshot = order.ProductTitleSnapshot
            };
            
            return orderDto;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> AddAsync(OrderAddDto dto)
    {
        var order = new Order()
        {
            ProductId = dto.ProductId,
            SellerId = dto.SellerId,
            BuyerId = dto.BuyerId,
            PriceAtPurchase = dto.PriceAtPurchase,
            Quantity = dto.Quantity,
            ProductTitleSnapshot = dto.ProductTitleSnapshot,
        };
        
        await context.Database.BeginTransactionAsync();
        try
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
            
            await context.Database.CommitTransactionAsync();
            
            return true;
        }
        catch
        {
            await context.Database.RollbackTransactionAsync();
            
            return false;
        }
    }

    public async Task<bool> RemoveAsync(Guid orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.Status != OrderStatus.Canceled);
        if (order is not null)
        {
            await context.Database.BeginTransactionAsync();
            try
            {
                order.Status = OrderStatus.Canceled;
                context.Orders.Update(order);
                await context.SaveChangesAsync();
                
                await context.Database.CommitTransactionAsync();
                
                return true;
            }
            catch
            {
                await context.Database.RollbackTransactionAsync();
                
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}