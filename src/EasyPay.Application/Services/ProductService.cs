using EasyPay.Application.DTOs.Product;
using EasyPay.Application.Interfaces;
using EasyPay.Core.Entities;
using EasyPay.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Application.Services;

public class ProductService(EPContext context)
    : IProductService
{
    public async Task<List<ProductGetDto>> GetAllActiveProductsAsync()
    {
        var products = await context.Products
            .Where(p => p.IsDeleted == false).ToListAsync();
        List<ProductGetDto> productsDto = new List<ProductGetDto>();
        foreach (var product in products)
        {
            productsDto.Add(new ProductGetDto()
            {
                Title = product.Title,
                Description = product.Description,
                CurrentPrice =  product.CurrentPrice,
                StockQuantity = product.StockQuantity
            });
        }
        
        return productsDto;
    }

    public async Task<ProductGetDto> GetActiveProductByIdAsync(Guid productId)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);
        if (product is not null)
        {
            ProductGetDto productDto = new ProductGetDto()
            {
                Title = product.Title,
                Description = product.Description,
                CurrentPrice = product.CurrentPrice,
                StockQuantity = product.StockQuantity
            };
            
            return productDto;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> AddAsync(ProductAddDto dto)
    {
        var product = new Product()
        {
            Title = dto.Title,
            Description = dto.Description,
            CurrentPrice = dto.CurrentPrice,
            StockQuantity = dto.StockQuantity,
            SellerId = dto.SellerId,
            GameId = dto.GameId
        };
        
        await context.Database.BeginTransactionAsync();
        try
        {
            await context.Products.AddAsync(product);
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

    public async Task<bool> RemoveAsync(Guid productId)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);
        if (product is not null)
        {
            await context.Database.BeginTransactionAsync();
            try
            {
                product.IsDeleted = true;
                context.Products.Update(product);
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