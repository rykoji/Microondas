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

}