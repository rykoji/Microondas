
namespace Microondas.Domain;

public class Microondas
{

    public int Seconds { get; private set; }

    public void AdicionarTempo(int seconds)
    {
        if (seconds < 1 || seconds > 120) throw new Exception("Valor do tempo fora do limite permitido");

        Seconds = seconds;
    }
}
