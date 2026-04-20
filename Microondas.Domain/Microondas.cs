
namespace Microondas.Domain;

public class Microondas
{
    public Microondas()
    {
        SetDefaultMicroondas();
    }

    public int Seconds { get; private set; }

    public int PowerLevel { get; private set; }
    public bool EstaAquecendo { get; set; }

    public bool _usandoProgramaPreDefinido = false;

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

        if (EstaAquecendo)
        {
            if (_usandoProgramaPreDefinido)
                throw new Exception("Não é permitido acrescentar tempo em programas pré-definidos");

            Seconds += 30;
            return;
        }

        if (Seconds <= 0) throw new Exception("Valor de tempo invalido");

        EstaAquecendo = true;

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

    public async Task StartWithAquecimento(IAquecimento aquecimento)
    {
        _usandoProgramaPreDefinido = !aquecimento.IsCustomize;
        Seconds = aquecimento.Seconds;
        PowerLevel = aquecimento.PowerLevel;
        await Start();

    }



    public void Stop()
    {
        if (!EstaAquecendo)
        {
            SetDefaultMicroondas();
            return;
        }
        _cts?.Cancel();
        EstaAquecendo = false;
    }

    private void SetDefaultMicroondas()
    {
        Seconds = DEFAULT_TIMER_LEVEL;
        PowerLevel = DEFAULT_POWER_LEVEL;
    }

    private static int DEFAULT_POWER_LEVEL => 10;
    private static int DEFAULT_TIMER_LEVEL => 30;
}
