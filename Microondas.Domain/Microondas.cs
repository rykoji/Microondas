
namespace Microondas.Domain;

public class Microondas
{

    public int Seconds { get; private set; }

    public int PowerLevel { get; private set; } = DEFAULT_POWER_LEVEL;
    public void AdicionarTempo(int seconds)
    {
        if (seconds < 1 || seconds > 120) throw new Exception("Valor do tempo fora do limite permitido");

        Seconds = seconds;
    }

    public void SelecionarPotencia(int powerLevel)
    {
        if (powerLevel < 1 || powerLevel > 10) throw new Exception("Valor de potencia fora do limite permitido");

        PowerLevel = powerLevel;
    }
    private static int DEFAULT_POWER_LEVEL => 10;
}
