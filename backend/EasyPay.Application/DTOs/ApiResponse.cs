namespace EasyPay.Application.DTOs;

public class ApiResponse<T>
{
    public bool IsError { get; set; } = false;
    
    public string ErrorMessage { get; set; } = string.Empty;
    
    public T Data { get; set; }

    public ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            Data = data
        };
    }

    public ApiResponse<T> Error(string errorMessage)
    {
        return new ApiResponse<T>
        {
            IsError = true,
            ErrorMessage = errorMessage,
            Data = default(T)
        };
    }
}