
using Microondas.Domain.Exceptions;

namespace Microondas.Domain;

public interface IMicroondasTimer
{
    int RemainingSeconds { get; }
    event Action? OnTick;
    event Action? OnFinished;
    Task StartAsync(int durationSeconds);
    Task AddTime(int seconds);
    void Stop();
    Task Continue();
    void Reset();
}

public class MicroondasTimer : IMicroondasTimer
{
    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;
    
    public int RemainingSeconds { get; private set; }
    public event Action? OnTick;
    public event Action? OnFinished;

    private bool IsRunngin { get; set; }



    public async Task StartAsync(int durationSeconds)
    {
        RemainingSeconds = durationSeconds;
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        try
        {
            while (IsRunngin = await _timer.WaitForNextTickAsync(_cts.Token))
            {
                RemainingSeconds--;
                OnTick?.Invoke();

                if (RemainingSeconds <= 0)
                {
                    _cts?.Cancel();
                    OnFinished?.Invoke();
                    break;
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
    }

    public async Task AddTime(int seconds)
    {
        if (RemainingSeconds + seconds >= 120)
        {
            RemainingSeconds = 120;
            return;
        }
            RemainingSeconds += seconds;
    }

    public async Task Continue()
    {
        await StartAsync(RemainingSeconds);
    }

    public void Reset()
    {
        Stop();
        RemainingSeconds = 30;
    }
}

public class Microondas
{
    private readonly IMicroondasTimer _timerProvider;

    private Microondas(IMicroondasTimer timerProvider)
    {
        _timerProvider = timerProvider;
        _timerProvider.OnTick += () => 
        {
            Seconds = _timerProvider.RemainingSeconds;
            OnTick?.Invoke();
        };
        _timerProvider.OnFinished += () =>
        {
            EstaAquecendo = false;
            OnFinished?.Invoke();
        };
        SetDefaultMicroondas();
    }

    public static Microondas Criar()
    {
        return new Microondas(new MicroondasTimer());
    }

    public static Microondas Criar(IMicroondasTimer timer)
    {
        return new Microondas(timer);
    }

    public int Seconds { get; private set; }

    public int PowerLevel { get; private set; }
    public bool EstaAquecendo { get; private set; }
    public char CaracterAquecimento { get; private set; } = '.';

    public bool usandoProgramaPreDefinido = false;

    public event Action? OnTick;
    public event Action? OnFinished;

    public void AdicionarTempo(int seconds)
    {
        if (seconds < 1 || seconds > 120) throw new DomainException("Valor do tempo fora do limite permitido");

        Seconds = seconds;
    }

    public void SelecionarPotencia(int powerLevel)
    {
        if (powerLevel < 1 || powerLevel > 10) throw new DomainException("Valor de potencia fora do limite permitido");

        PowerLevel = powerLevel;
    }

    public void ResetarCaracterAquecimento()
    {
        CaracterAquecimento = '.';
        usandoProgramaPreDefinido = false;
    }

    public async Task Start()
    {
        if (EstaAquecendo)
        {
            if (usandoProgramaPreDefinido)
                throw new DomainException("Não é permitido acrescentar tempo em programas pré-definidos");

            await _timerProvider.AddTime(30);
            return;
        }

        if (Seconds <= 0) throw new DomainException("Valor de tempo invalido");

        EstaAquecendo = true;
        if (_timerProvider.RemainingSeconds <= 0)
            await _timerProvider.StartAsync(Seconds);
        else
            await _timerProvider.Continue();
    }

    public async Task StartWithAquecimento(IAquecimento aquecimento)
    {
        usandoProgramaPreDefinido = !aquecimento.IsCustomize;
        Seconds = aquecimento.Seconds;
        PowerLevel = aquecimento.PowerLevel;
        CaracterAquecimento = aquecimento.CaracterAquecimento;
        await Start();
    }



    public void Stop()
    {
        if (!EstaAquecendo)
        {
            SetDefaultMicroondas();
            _timerProvider.Reset();
            return;
        }
        _timerProvider.Stop();
        EstaAquecendo = false;
    }

    private void SetDefaultMicroondas()
    {
        Seconds = DEFAULT_TIMER_LEVEL;
        PowerLevel = DEFAULT_POWER_LEVEL;
        CaracterAquecimento = '.';
        usandoProgramaPreDefinido = DEFAULT_WARNING;
    }

    private static int DEFAULT_POWER_LEVEL => 10;
    private static int DEFAULT_TIMER_LEVEL => 30;
    private static bool DEFAULT_WARNING => false;
}
