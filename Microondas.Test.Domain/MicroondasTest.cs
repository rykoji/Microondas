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
}