namespace EasyPay.Application.Interfaces;

public interface IPasswordHasher
{
    string Encrypt(string password);
    bool Verify(string password);
    string Decrypt(string password);
}