namespace CareFusion.Dispensing.Services.Cryptography
{
    public interface ITextEncryptor
    {
        bool SupportsSalt { get; }

        string GenerateHash(string text, string salt);

        bool IsMatch(string text, string salt, string hash);
    }
}
