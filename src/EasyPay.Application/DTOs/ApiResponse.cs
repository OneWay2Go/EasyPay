namespace EasyPay.Application.DTOs;

public class ApiResponse<T>
{
    public bool IsError { get; set; } = false;
    
    public string ErrorMessage { get; set; } = string.Empty;
    
    public T Data { get; set; }
}