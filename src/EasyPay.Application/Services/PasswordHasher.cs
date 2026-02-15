using EasyPay.Application.Interfaces;

namespace EasyPay.Application.Services;

public class PasswordHasher : IPasswordHasher
{
    public string Encrypt(string password)
    {
        throw new NotImplementedException();
    }

    public bool Verify(string password)
    {
        throw new NotImplementedException();
    }

    public string Decrypt(string password)
    {
        throw new NotImplementedException();
    }
}