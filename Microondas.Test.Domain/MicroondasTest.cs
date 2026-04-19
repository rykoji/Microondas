namespace Microondas.Test.Domain;

public class MicroondasTest
{
    [Fact]
    public void AcionarTempo_ValorDentroTempoLimite()
    {
        var microondas = new Microondas.Domain.Microondas();

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
        var microondas = new Microondas.Domain.Microondas();
        Assert.Throws<Exception>(() => microondas.AdicionarTempo(segundosForaDoIntervalo));

    }

    [Fact]
    public void SelecionarPotencia_ValorDentroTempoLimite()
    {
        var microondas = new Microondas.Domain.Microondas();

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
        var microondas = new Microondas.Domain.Microondas();
        Assert.Throws<Exception>(() => microondas.SelecionarPotencia(potenciaForaDoIntervalo));

    }


    [Fact]
    public async Task Start_SemTempoDefinido()
    {
        var microondas = new Microondas.Domain.Microondas();
        await Assert.ThrowsAsync<Exception>(() => microondas.Start());
    }

    [Fact]
    public async Task Start_SemPotenciaDefinida()
    {
        var microondas = new Microondas.Domain.Microondas();
        microondas.AdicionarTempo(1);
        await microondas.Start();

        Assert.Equal(10, microondas.PowerLevel);
    }
    
    [Fact]
    public void Start_InicioRapido()
    {
        var microondas = new Microondas.Domain.Microondas();

        Assert.Equal(30, microondas.Seconds);
        Assert.Equal(10, microondas.PowerLevel);
    }

    [Fact]
    public async Task Stop_PausaDuranteAquecimento()
    {
        var microondas = new Microondas.Domain.Microondas();
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
        var microondas = new Microondas.Domain.Microondas();
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
        var microondas = new Microondas.Domain.Microondas();
        microondas.AdicionarTempo(60);
        microondas.SelecionarPotencia(1);

        microondas.Stop();

        Assert.Equal(30, microondas.Seconds);
        Assert.Equal(10, microondas.PowerLevel);
    }
}