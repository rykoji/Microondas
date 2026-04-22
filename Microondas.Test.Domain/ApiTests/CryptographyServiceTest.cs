using Microondas.WebApplication.Services;

namespace Microondas.Test.Domain.ApiTests;

public class CryptographyServiceTest
{
    [Fact]
    public void HashSHA256_MesmosInputsRetornamMesmoHash()
    {
        var hash1 = CryptographyService.HashSHA256("admin123");
        var hash2 = CryptographyService.HashSHA256("admin123");
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashSHA256_InputsDiferentesRetornamHashesDiferentes()
    {
        var hash1 = CryptographyService.HashSHA256("senha1");
        var hash2 = CryptographyService.HashSHA256("senha2");
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashSHA256_RetornaStringNaoVazia()
    {
        var hash = CryptographyService.HashSHA256("qualquervalor");
        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void Encrypt_Decrypt_RoundTrip()
    {
        const string original = "Server=meuserver;Database=MicroondasDb;";
        const string key = "MinhaChaveDeTeste2024!SecretPass";

        var encrypted = CryptographyService.Encrypt(original, key);
        var decrypted = CryptographyService.Decrypt(encrypted, key);

        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void Encrypt_MesmosInputsRetornamMesmoOutput()
    {
        const string plain = "texto_para_criptografar";
        const string key = "chave_de_teste";

        var enc1 = CryptographyService.Encrypt(plain, key);
        var enc2 = CryptographyService.Encrypt(plain, key);

        Assert.Equal(enc1, enc2);
    }

    [Fact]
    public void Encrypt_RetornaStringBase64Valida()
    {
        var encrypted = CryptographyService.Encrypt("texto", "chave");

        Assert.False(string.IsNullOrEmpty(encrypted));
        var bytes = Convert.FromBase64String(encrypted);
        Assert.NotEmpty(bytes);
    }

    [Fact]
    public void Decrypt_ComChaveErrada_LancaExcecao()
    {
        var encrypted = CryptographyService.Encrypt("texto", "chave-correta-32chars-padding!!");
        Assert.ThrowsAny<Exception>(() =>
            CryptographyService.Decrypt(encrypted, "chave-errada-32chars-padding!!XX"));
    }
}
