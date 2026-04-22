using Microondas.WebApplication.Controllers;
using Microondas.WebApplication.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Microondas.Test.Domain.ApiTests;

public class MicroondasControllerTest
{
    private static void ResetarEstaticoMicroondas() =>
        typeof(MicroondasController)
            .GetField("_microondas", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, null);

    [Fact]
    public void Iniciar_SemParametros_RetornaOk()
    {
        var controller = new MicroondasController();

        var result = controller.Iniciar(null, null);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Iniciar_SemParametros_UsaValoresPadrao()
    {
        var controller = new MicroondasController();

        var result = (OkObjectResult)controller.Iniciar(null, null);
        var props = result.Value!.GetType().GetProperties();
        var tempo = (int)props.First(p => p.Name == "Tempo").GetValue(result.Value)!;
        var potencia = (int)props.First(p => p.Name == "Potencia").GetValue(result.Value)!;

        Assert.Equal(30, tempo);
        Assert.Equal(10, potencia);
    }

    [Fact]
    public void Iniciar_ComTempoEPotencia_RetornaValoresInformados()
    {
        var controller = new MicroondasController();

        var result = (OkObjectResult)controller.Iniciar(60, 7);
        var props = result.Value!.GetType().GetProperties();
        var tempo = (int)props.First(p => p.Name == "Tempo").GetValue(result.Value)!;
        var potencia = (int)props.First(p => p.Name == "Potencia").GetValue(result.Value)!;

        Assert.Equal(60, tempo);
        Assert.Equal(7, potencia);
    }

    [Fact]
    public void Parar_SemMicroondasIniciado_LancaBusinessException()
    {
        ResetarEstaticoMicroondas();
        var controller = new MicroondasController();

        Assert.Throws<BusinessException>(() => controller.Parar());
    }

    [Fact]
    public void Parar_ComMicroondasIniciado_RetornaOk()
    {
        var controller = new MicroondasController();
        controller.Iniciar(30, null);

        var result = controller.Parar();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Status_SemMicroondasIniciado_RetornaOkComMensagem()
    {
        ResetarEstaticoMicroondas();
        var controller = new MicroondasController();

        var result = controller.Status();
        var ok = Assert.IsType<OkObjectResult>(result);
        var props = ok.Value!.GetType().GetProperties();
        var mensagem = props.FirstOrDefault(p => p.Name == "Message")?.GetValue(ok.Value);

        Assert.NotNull(mensagem);
    }

    [Fact]
    public void Status_ComMicroondasIniciado_RetornaStatus()
    {
        var controller = new MicroondasController();
        controller.Iniciar(30, null);

        var result = controller.Status();
        var ok = Assert.IsType<OkObjectResult>(result);
        var props = ok.Value!.GetType().GetProperties();
        var estaAquecendo = props.FirstOrDefault(p => p.Name == "EstaAquecendo");

        Assert.NotNull(estaAquecendo);
    }
}
