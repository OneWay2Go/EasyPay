using EasyPay.Application.DTOs;
using EasyPay.Application.DTOs.Order;
using EasyPay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController(
    IOrderService orderService
    ) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var ordersDto = await orderService.GetAllAsync();
        return Ok(new ApiResponse<List<OrderGetDto>>().Success(ordersDto));
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] Guid orderId)
    {
        var order = await orderService.GetByIdAsync(orderId);
        if (order is not null)
        {
            return Ok(new ApiResponse<OrderGetDto>().Success(order));
        }
        else
        {
            return NotFound(new ApiResponse<string>().Error("Order not found"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] OrderAddDto orderAddDto)
    {
        bool isAdded = await orderService.AddAsync(orderAddDto);
        if (isAdded)
        {
            return Ok(new ApiResponse<string>().Success("Order added"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Success("Order not added"));
        }
    }

    [HttpPut("delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid orderId)
    {
        bool isDeleted = await orderService.RemoveAsync(orderId);
        if (isDeleted)
        {
            return Ok(new ApiResponse<string>().Success("Order deleted"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Success("Order not deleted"));
        }
    }
}