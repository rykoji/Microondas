namespace Microondas.WebApplication.Services;

public class EncryptedConnectionStringProvider : IConnectionStringProvider
{
    private readonly string _encryptedValue;
    private readonly string _key;

    public EncryptedConnectionStringProvider(string encryptedValue, string key)
    {
        _encryptedValue = encryptedValue;
        _key = key;
    }

    public string GetConnectionString() =>
        CryptographyService.Decrypt(_encryptedValue, _key);
}
