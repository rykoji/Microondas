
namespace Microondas.Domain;

public class Microondas
{

    public int Seconds { get; private set; } = DEFAULT_TIMER_LEVEL;

    public int PowerLevel { get; private set; } = DEFAULT_POWER_LEVEL;
    public bool EstaAquecendo { get; set; }

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;
    public event Action? OnTick;
    public event Action? OnFinished;

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

    public async Task Start()
    {
        if (Seconds <= 0) throw new Exception("Valor de tempo invalido");
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        try
        {
            while (EstaAquecendo = await _timer.WaitForNextTickAsync(_cts.Token))
            {
                Seconds--;
                OnTick?.Invoke();

                if (Seconds <= 0)
                {
                    EstaAquecendo = false;
                    OnFinished?.Invoke();
                    Stop();

                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }



    public void Stop()
    {
        _cts?.Cancel();
        EstaAquecendo = false;
    }

    private static int DEFAULT_POWER_LEVEL => 10;
    private static int DEFAULT_TIMER_LEVEL => 30;
}
