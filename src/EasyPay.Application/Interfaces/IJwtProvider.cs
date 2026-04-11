using EasyPay.Core.Entities;

namespace EasyPay.Application.Interfaces
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }
}
