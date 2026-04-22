using Microondas.WebApplication.Controllers;
using Microondas.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Microondas.Test.Domain.ApiTests;

public class AuthControllerTest
{
    private static AuthController CriarController()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "A458F2FB-6F76-43CC-8279-0409CE70CC83",
                ["Jwt:Issuer"] = "MicroondasAPI",
                ["Jwt:Audience"] = "MicroondasClient",
                ["Jwt:ExpireMinutes"] = "60"
            })
            .Build();
        return new AuthController(config);
    }

    [Fact]
    public void Login_CredenciaisValidas_RetornaOk()
    {
        var controller = CriarController();

        var result = controller.Login(new LoginRequest { Username = "admin", Password = "admin123" });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Login_CredenciaisValidas_RetornaTokenNaoVazio()
    {
        var controller = CriarController();

        var result = (OkObjectResult)controller.Login(new LoginRequest { Username = "admin", Password = "admin123" });
        var response = Assert.IsType<LoginResponse>(result.Value);

        Assert.False(string.IsNullOrEmpty(response.Token));
    }

    [Fact]
    public void Login_CredenciaisValidas_TokenComExpiracao()
    {
        var controller = CriarController();

        var result = (OkObjectResult)controller.Login(new LoginRequest { Username = "admin", Password = "admin123" });
        var response = Assert.IsType<LoginResponse>(result.Value);

        Assert.True(response.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public void Login_SenhaErrada_RetornaUnauthorized()
    {
        var controller = CriarController();

        var result = controller.Login(new LoginRequest { Username = "admin", Password = "senha_errada" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Login_UsuarioErrado_RetornaUnauthorized()
    {
        var controller = CriarController();

        var result = controller.Login(new LoginRequest { Username = "usuario_invalido", Password = "admin123" });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
