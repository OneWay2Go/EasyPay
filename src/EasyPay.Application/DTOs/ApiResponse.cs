namespace EasyPay.Application.DTOs;

public class ApiResponse<T>
{
    private bool IsError { get; set; } = false;
    
    private string ErrorMessage { get; set; } = string.Empty;
    
    private T? Data { get; set; }

    public ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            Data = data
        };
    }

    public ApiResponse<T?> Error(string errorMessage)
    {
        return new ApiResponse<T?>
        {
            IsError = true,
            ErrorMessage = errorMessage,
        };
    }
}