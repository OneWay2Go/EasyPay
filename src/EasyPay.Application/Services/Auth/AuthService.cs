using EasyPay.Application.Interfaces;

namespace EasyPay.Application.Services.Auth
{
    public class AuthService(
        IUserService userService,
        IJwtProvider jwtProvider
        ) : IAuthService
    {
        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await userService.GetActiveUserByUsernameAsync(username);
            if (user is not null)
            {
                var isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash);
                if (isPasswordValid)
                {
                    string token = jwtProvider.Generate(user);

                    return token;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
