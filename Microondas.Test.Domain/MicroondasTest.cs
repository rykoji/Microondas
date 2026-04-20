using Microondas.Domain;
using Microondas.Domain.Entities;
using Microondas.Domain.Services;
using Microondas.Infrastructure.Data;
using Microondas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Microondas.Test.Domain;

public class MicroondasTest
{
    [Fact]
    public void AcionarTempo_ValorDentroTempoLimite()
    {
        var microondas = Microondas.Domain.Microondas.Criar();

        microondas.AdicionarTempo(1);

        Assert.Equal(1, microondas.Seconds);
    }

    [Theory]
    [InlineData(121)]
    [InlineData(500)]
    [InlineData(0)]
    [InlineData(-1)]
    public void AcionarTempo_ValorAcimaDoLimite(int segundosForaDoIntervalo)
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        Assert.Throws<Exception>(() => microondas.AdicionarTempo(segundosForaDoIntervalo));

    }

    [Fact]
    public void SelecionarPotencia_ValorDentroTempoLimite()
    {
        var microondas = Microondas.Domain.Microondas.Criar();

        microondas.SelecionarPotencia(1);

        Assert.Equal(1, microondas.PowerLevel);
    }


    [Theory]
    [InlineData(17)]
    [InlineData(47)]
    [InlineData(-1)]
    [InlineData(0)]
    public void SelecionarPotencia_ForaLimite(int potenciaForaDoIntervalo)
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        Assert.Throws<Exception>(() => microondas.SelecionarPotencia(potenciaForaDoIntervalo));

    }

    [Fact]
    public async Task Start_SemTempoDefinido()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        Assert.Equal(30, microondas.Seconds);
    }

    [Fact]
    public async Task Start_SemPotenciaDefinida()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        microondas.AdicionarTempo(4);
        await microondas.Start();

        Assert.Equal(10, microondas.PowerLevel);
    }

    [Fact]
    public void Start_InicioRapido()
    {
        var microondas = Microondas.Domain.Microondas.Criar();

        Assert.Equal(30, microondas.Seconds);
        Assert.Equal(10, microondas.PowerLevel);
    }

    [Fact]
    public async Task Stop_PausaDuranteAquecimento()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        microondas.AdicionarTempo(3);

        _ = microondas.Start();
        await Task.Delay(100);

        microondas.Stop();

        Assert.False(microondas.EstaAquecendo);
        Assert.True(microondas.Seconds > 0);
    }

    [Fact]
    public async Task Stop_CancelarQuandoPausado()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        microondas.AdicionarTempo(10);

        _ = microondas.Start();
        await Task.Delay(100);

        microondas.Stop();
        microondas.Stop();

        Assert.Equal(30, microondas.Seconds);
        Assert.Equal(10, microondas.PowerLevel);
    }

    [Fact]
    public async Task Stop_LimparInformacoes()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        microondas.AdicionarTempo(60);
        microondas.SelecionarPotencia(1);

        microondas.Stop();

        Assert.Equal(30, microondas.Seconds);
        Assert.Equal(10, microondas.PowerLevel);
    }

    [Fact]
    public async Task Start_AcrescentarTempoDuranteAquecimento()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        microondas.AdicionarTempo(60);

        _ = microondas.Start();
        await Task.Delay(100);

        var tempoAntes = microondas.Seconds;
        _ = microondas.Start();

        Assert.Equal(tempoAntes + 30, microondas.Seconds);

        microondas.Stop();

    }
    
    [Fact]
    public async Task Start_DisparaOnTickCadaSegundo()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        microondas.AdicionarTempo(2);

        var tickCount = 0;
        microondas.OnTick += () => tickCount++;

        await microondas.Start();
        Assert.Equal(2, tickCount);
    }

    [Theory]
    [InlineData("Pipoca", 180, 7)]
    [InlineData("Leite", 300, 5)]
    [InlineData("Carne", 840, 4)]
    [InlineData("Frango", 480, 7)]
    [InlineData("Feijao", 480, 9)]
    public void ProgramasPreDefinidos_DevemTerValoresCorretos(string nome, int tempoEsperado, int potenciaEsperada)
    {
        var programas = new Dictionary<string, Microondas.Domain.IAquecimento>
        {
            { "Pipoca", new Microondas.Console.PipocaAquecimento() },
            { "Leite", new Microondas.Console.LeiteAquecimento() },
            { "Carne", new Microondas.Console.CarneAquecimento() },
            { "Frango", new Microondas.Console.FrangoAquecimento() },
            { "Feijao", new Microondas.Console.FeijaoAquecimento() }
        };

        var programa = programas[nome];

        Assert.Equal(tempoEsperado, programa.Seconds);
        Assert.Equal(potenciaEsperada, programa.PowerLevel);
    }

    [Fact]
    public async Task StartWithAquecimento_NaoPermitirAcrescimoEmPreDefinido()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        var pipoca = new Microondas.Console.PipocaAquecimento();
    
        _ = microondas.StartWithAquecimento(pipoca);
        await Task.Delay(100);
    
        var tempoAntes = microondas.Seconds;
        
        Assert.ThrowsAsync<Exception>(() => microondas.Start());
    
        microondas.Stop();
    }

    [Fact]
    public async Task StartWithAquecimento_DeveCarregarValoresDoPrograma()
    {
        var microondas = Microondas.Domain.Microondas.Criar();
        var pipoca = new Microondas.Console.PipocaAquecimento();

        _ = microondas.StartWithAquecimento(pipoca);
        await Task.Delay(100);

        Assert.Equal(7, microondas.PowerLevel);
        Assert.True(microondas.Seconds <= 180 && microondas.Seconds > 0);

        microondas.Stop();
    }

    [Fact]
    public void AquecimentoCustomizado_DeveSerIdentificadoComoCustomizado()
    {
        var customizado = new AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '*'
        };

        Assert.True(customizado.IsCustomize);
    }

    [Fact]
    public void AquecimentoCustomizado_CamposObrigatorios_NomeVazio()
    {
        var customizado = new AquecimentoCustomizado
        {
            Nome = "",
            Alimento = "Arroz",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '*'
        };

        var programasExistentes = new List<IAquecimento>();
        var validator = new AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoCustomizado_CaractereNaoPodeSerPonto()
    {
        var customizado = new AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '.'
        };

        var programasExistentes = new List<IAquecimento>();
        var validator = new AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoCustomizado_CaractereNaoPodeRepetir()
    {
        var programaExistente = new AquecimentoCustomizado
        {
            Nome = "Existente",
            CaracterAquecimento = '*'
        };

        var customizado = new AquecimentoCustomizado
        {
            Nome = "Novo",
            Alimento = "Algo",
            Seconds = 60,
            PowerLevel = 5,
            CaracterAquecimento = '*'
        };

        var programasExistentes = new List<IAquecimento> { programaExistente };
        var validator = new AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoCustomizado_InstrucoesOpcional()
    {
        var customizado = new AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '*',
            Instrucoes = null
        };

        var programasExistentes = new List<IAquecimento>();
        var validator = new AquecimentoValidator(programasExistentes);

        var exception = Record.Exception(() => validator.Validar(customizado));
        Assert.Null(exception);
    }

    [Fact]
    public void AquecimentoCustomizado_ProgramaPreDefinidoNaoEhCustomizado()
    {
        var pipoca = new Microondas.Console.PipocaAquecimento();
        Assert.False(pipoca.IsCustomize);
    }

    [Fact]
    public void AquecimentoValidator_CamposObrigatorios_NomeVazio()
    {
        var customizado = new Microondas.Domain.Entities.AquecimentoCustomizado
        {
            Nome = "",
            Alimento = "Arroz",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '*'
        };

        var programasExistentes = new List<Microondas.Domain.IAquecimento>();
        var validator = new Microondas.Domain.Services.AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoValidator_CamposObrigatorios_AlimentoVazio()
    {
        var customizado = new Microondas.Domain.Entities.AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '*'
        };

        var programasExistentes = new List<Microondas.Domain.IAquecimento>();
        var validator = new Microondas.Domain.Services.AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoValidator_CaractereNaoPodeSerPonto()
    {
        var customizado = new Microondas.Domain.Entities.AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '.'
        };

        var programasExistentes = new List<Microondas.Domain.IAquecimento>();
        var validator = new Microondas.Domain.Services.AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoValidator_CaractereNaoPodeRepetir()
    {
        var programaExistente = new Microondas.Console.PipocaAquecimento();

        var customizado = new Microondas.Domain.Entities.AquecimentoCustomizado
        {
            Nome = "Novo",
            Alimento = "Algo",
            Seconds = 60,
            PowerLevel = 5,
            CaracterAquecimento = 'P'
        };

        var programasExistentes = new List<Microondas.Domain.IAquecimento> { programaExistente };
        var validator = new Microondas.Domain.Services.AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoValidator_InstrucoesOpcional()
    {
        var customizado = new Microondas.Domain.Entities.AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '*',
            Instrucoes = null
        };

        var programasExistentes = new List<Microondas.Domain.IAquecimento>();
        var validator = new Microondas.Domain.Services.AquecimentoValidator(programasExistentes);

        var exception = Record.Exception(() => validator.Validar(customizado));
        Assert.Null(exception);
    }

    [Fact]
    public void AquecimentoValidator_PotenciaForaDoLimite()
    {
        var customizado = new Microondas.Domain.Entities.AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 300,
            PowerLevel = 15,
            CaracterAquecimento = '*'
        };

        var programasExistentes = new List<Microondas.Domain.IAquecimento>();
        var validator = new Microondas.Domain.Services.AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public void AquecimentoValidator_TempoZero()
    {
        var customizado = new Microondas.Domain.Entities.AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 0,
            PowerLevel = 8,
            CaracterAquecimento = '*'
        };

        var programasExistentes = new List<Microondas.Domain.IAquecimento>();
        var validator = new Microondas.Domain.Services.AquecimentoValidator(programasExistentes);

        Assert.Throws<ArgumentException>(() => validator.Validar(customizado));
    }

    [Fact]
    public async Task CadastrarProgramaCustomizado_Sucesso()
    {
        var options = new DbContextOptionsBuilder<MicroondasDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        using var context = new MicroondasDbContext(options);
        var repository = new AquecimentoSqlRepository(context);
        var service = new AquecimentoService(repository, new List<IAquecimento>());

        var customizado = new AquecimentoCustomizado
        {
            Nome = "Arroz",
            Alimento = "Arroz branco",
            Seconds = 300,
            PowerLevel = 8,
            CaracterAquecimento = '*',
            Instrucoes = ""
        };

        await service.CadastrarAsync(customizado);

        var resultado = await repository.ObterPorNomeAsync("Arroz");
        Assert.NotNull(resultado);
        Assert.Equal("Arroz", resultado.Nome);
    }

}