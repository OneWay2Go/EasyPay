using EasyPay.Application.DTOs;
using EasyPay.Application.DTOs.Product;
using EasyPay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(
        IProductService productService
    ) : ControllerBase
{
    [HttpGet("get-all-active-products")]
    public async Task<IActionResult> GetAll()
    {
        var productsDto = await productService.GetAllActiveProductsAsync();
        return Ok(new ApiResponse<List<ProductGetDto>>().Success(productsDto));
    }

    [HttpGet("get-active-product-by-id")]
    public async Task<IActionResult> GetById([FromQuery] Guid productId)
    {
        var product = await productService.GetActiveProductByIdAsync(productId);
        if (product is not null)
        {
            return Ok(new ApiResponse<ProductGetDto>().Success(product));
        }
        else
        {
            return NotFound(new ApiResponse<ProductGetDto>().Error("Product not found"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] ProductAddDto dto)
    {
        bool isAdded = await productService.AddAsync(dto);
        if (isAdded)
        {
            return Ok(new ApiResponse<string>().Success("Product added"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Error("Product not added"));
        }
    }

    [HttpPut("delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid productId)
    {
        bool isDeleted = await productService.RemoveAsync(productId);
        if (isDeleted)
        {
            return Ok(new ApiResponse<string>().Success("Product deleted"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Error("Product not deleted, or it is already deleted"));
        }
    }
}