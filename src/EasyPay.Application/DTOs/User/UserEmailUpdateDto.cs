namespace EasyPay.Application.DTOs.User;

public class UserEmailUpdateDto
{
    public string Username { get; set; }
    public string CurrentEmail { get; set; }
    public string NewEmail { get; set; }
}