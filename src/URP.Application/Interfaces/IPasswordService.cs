namespace URP.Application.Interfaces;

public interface IPasswordService
{
    string Hash(string plainPassword);
    bool   Verify(string plainPassword, string hash);
}
