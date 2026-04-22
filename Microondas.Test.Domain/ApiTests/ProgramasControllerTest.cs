using Microondas.Domain;
using Microondas.Domain.Entities;
using Microondas.Domain.Services;
using Microondas.Infrastructure.Data;
using Microondas.Infrastructure.Repositories;
using Microondas.WebApplication.Controllers;
using Microondas.WebApplication.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Microondas.Test.Domain.ApiTests;

public class ProgramasControllerTest
{
    private static ProgramasController CriarController()
    {
        var options = new DbContextOptionsBuilder<MicroondasDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new MicroondasDbContext(options);
        var repo = new AquecimentoSqlRepository(context);
        var programas = new List<IAquecimento>
        {
            new PipocaAquecimento(),
            new LeiteAquecimento(),
            new CarneAquecimento(),
            new FrangoAquecimento(),
            new FeijaoAquecimento()
        };
        var service = new AquecimentoService(repo, programas);
        return new ProgramasController(service);
    }

    [Fact]
    public async Task ObterTodos_RetornaOsСincoProgramasPreDefinidos()
    {
        var controller = CriarController();

        var result = await controller.ObterTodos();
        var ok = Assert.IsType<OkObjectResult>(result);
        var lista = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);

        Assert.Equal(5, lista.Count());
    }

    [Fact]
    public async Task ObterPorNome_ProgramaExistente_RetornaOk()
    {
        var controller = CriarController();

        var result = await controller.ObterPorNome("Pipoca");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObterPorNome_ProgramaInexistente_RetornaNotFound()
    {
        var controller = CriarController();

        var result = await controller.ObterPorNome("ProgramaQueNaoExiste");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Cadastrar_ProgramaValido_RetornaCreated()
    {
        var controller = CriarController();
        var programa = new AquecimentoCustomizado
        {
            Nome = "Meu Programa",
            Alimento = "Arroz",
            Seconds = 60,
            PowerLevel = 5,
            CaracterAquecimento = 'M'
        };

        var result = await controller.Cadastrar(programa);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Cadastrar_CaractereJaUsado_LancaBusinessException()
    {
        var controller = CriarController();
        var programa = new AquecimentoCustomizado
        {
            Nome = "Teste",
            Alimento = "Alimento",
            Seconds = 60,
            PowerLevel = 5,
            CaracterAquecimento = 'P' // 'P' já é da Pipoca
        };

        await Assert.ThrowsAsync<BusinessException>(() => controller.Cadastrar(programa));
    }

    [Fact]
    public async Task Remover_ProgramaPreDefinido_LancaBusinessException()
    {
        var controller = CriarController();

        await Assert.ThrowsAsync<BusinessException>(() => controller.Remover("Pipoca"));
    }

    [Fact]
    public async Task Remover_ProgramaInexistente_RetornaNotFound()
    {
        var controller = CriarController();

        var result = await controller.Remover("NaoExiste");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Remover_ProgramaCustomizado_RetornaNoContent()
    {
        var controller = CriarController();
        await controller.Cadastrar(new AquecimentoCustomizado
        {
            Nome = "Customizado",
            Alimento = "Alimento",
            Seconds = 30,
            PowerLevel = 3,
            CaracterAquecimento = 'X'
        });

        var result = await controller.Remover("Customizado");

        Assert.IsType<NoContentResult>(result);
    }
}
