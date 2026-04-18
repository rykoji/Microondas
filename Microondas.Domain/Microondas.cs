
namespace Microondas.Domain;

public class Microondas
{

    public int Seconds { get; private set; }

    public int PowerLevel { get; private set; } = DEFAULT_POWER_LEVEL;

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
            while (await _timer.WaitForNextTickAsync(_cts.Token))
            {
                Seconds--;
                OnTick?.Invoke();

                if (Seconds <= 0)
                {
                    OnFinished?.Invoke();
                    Stop();
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void Stop() => _cts?.Cancel();

    private static int DEFAULT_POWER_LEVEL => 10;
}
