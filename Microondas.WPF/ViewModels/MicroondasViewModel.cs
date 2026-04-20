using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microondas.Domain;
using Microondas.Domain.Entities;
using Microondas.Console;

namespace Microondas.WPF.ViewModels;

public class MicroondasViewModel : INotifyPropertyChanged
{
    private readonly Domain.Microondas _microondas;
    private readonly List<IAquecimento> _programas;

    private int _tempoInput = 30;
    private int _potenciaInput = 10;
    private string _animacaoTexto = "";
    private string _instrucoes = "Selecione um programa ou configure manualmente.";

    // Campos para aquecimento customizado
    private string _novoNome = "";
    private string _novoAlimento = "";
    private string _novoTempo = "";
    private string _novaPotencia = "";
    private string _novoCaracter = "";

    public MicroondasViewModel()
    {
        _microondas = new Domain.Microondas();

        _programas = new List<IAquecimento>
        {
            new PipocaAquecimento(),
            new LeiteAquecimento(),
            new CarneAquecimento(),
            new FrangoAquecimento(),
            new FeijaoAquecimento()
        };

        _microondas.OnTick += () =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(TempoFormatado));
                OnPropertyChanged(nameof(PowerLevel));
                AtualizarAnimacao();
            });
        };

        _microondas.OnFinished += () =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AnimacaoTexto = "PRONTO!";
                Instrucoes = "Aquecimento concluído!";
            });
        };

        IniciarCommand = new RelayCommand(Iniciar);
        PararCommand = new RelayCommand(Parar);
        SelecionarProgramaCommand = new RelayCommand<string>(SelecionarPrograma);
        SalvarCustomizadoCommand = new RelayCommand(SalvarCustomizado);
    }

    // Propriedades de Binding
    public string TempoFormatado
    {
        get
        {
            var min = _microondas.Seconds / 60;
            var seg = _microondas.Seconds % 60;
            return $"{min:D2}:{seg:D2}";
        }
    }

    public int PowerLevel => _microondas.PowerLevel;

    public int TempoInput
    {
        get => _tempoInput;
        set { _tempoInput = value; OnPropertyChanged(); }
    }

    public int PotenciaInput
    {
        get => _potenciaInput;
        set { _potenciaInput = value; OnPropertyChanged(); }
    }

    public string AnimacaoTexto
    {
        get => _animacaoTexto;
        set { _animacaoTexto = value; OnPropertyChanged(); }
    }

    public string Instrucoes
    {
        get => _instrucoes;
        set { _instrucoes = value; OnPropertyChanged(); }
    }

    public bool EstaAquecendo => _microondas.EstaAquecendo;

    // Propriedades para aquecimento customizado
    public string NovoNome
    {
        get => _novoNome;
        set { _novoNome = value; OnPropertyChanged(); }
    }

    public string NovoAlimento
    {
        get => _novoAlimento;
        set { _novoAlimento = value; OnPropertyChanged(); }
    }

    public string NovoTempo
    {
        get => _novoTempo;
        set { _novoTempo = value; OnPropertyChanged(); }
    }

    public string NovaPotencia
    {
        get => _novaPotencia;
        set { _novaPotencia = value; OnPropertyChanged(); }
    }

    public string NovoCaracter
    {
        get => _novoCaracter;
        set { _novoCaracter = value; OnPropertyChanged(); }
    }

    // Commands
    public ICommand IniciarCommand { get; }
    public ICommand PararCommand { get; }
    public ICommand SelecionarProgramaCommand { get; }
    public ICommand SalvarCustomizadoCommand { get; }

    private async void Iniciar()
    {
        try
        {
            _microondas.AdicionarTempo(TempoInput);
            _microondas.SelecionarPotencia(PotenciaInput);
            Instrucoes = "Aquecendo...";
            await _microondas.Start();
        }
        catch (Exception ex)
        {
            Instrucoes = $"Erro: {ex.Message}";
        }
    }

    private void Parar()
    {
        _microondas.Stop();
        AnimacaoTexto = "";
        Instrucoes = "Aquecimento interrompido.";
        OnPropertyChanged(nameof(TempoFormatado));
    }

    private async void SelecionarPrograma(string? nome)
    {
        if (nome == null) return;

        var programa = _programas.FirstOrDefault(p => p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
        if (programa != null)
        {
            Instrucoes = programa.Instrucoes;
            await _microondas.StartWithAquecimento(programa);
        }
    }

    private void AtualizarAnimacao()
    {
        if (_microondas.EstaAquecendo)
        {
            var caracteres = new string('.', _microondas.PowerLevel * 3);
            AnimacaoTexto = $"~ {caracteres} ~";
        }
    }

    private void SalvarCustomizado()
    {
        // Validações
        if (string.IsNullOrWhiteSpace(NovoNome))
        {
            Instrucoes = "Erro: Nome é obrigatório.";
            return;
        }

        if (!int.TryParse(NovoTempo, out int tempo) || tempo < 1 || tempo > 120)
        {
            Instrucoes = "Erro: Tempo deve ser entre 1 e 120 segundos.";
            return;
        }

        if (!int.TryParse(NovaPotencia, out int potencia) || potencia < 1 || potencia > 10)
        {
            Instrucoes = "Erro: Potência deve ser entre 1 e 10.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NovoCaracter))
        {
            Instrucoes = "Erro: Caracter é obrigatório.";
            return;
        }

        // Verificar se caracter já existe
        var caracterExistente = _programas.Any(p => p.CaracterAquecimento == NovoCaracter[0]);
        if (caracterExistente)
        {
            Instrucoes = "Erro: Este caracter já está em uso por outro programa.";
            return;
        }

        // Criar novo aquecimento customizado
        var novoAquecimento = new AquecimentoCustomizado
        {
            Nome = NovoNome,
            Alimento = NovoAlimento,
            Seconds = tempo,
            PowerLevel = potencia,
            CaracterAquecimento = NovoCaracter[0],
            Instrucoes = $"Programa customizado: {NovoNome}"
        };

        _programas.Add(novoAquecimento);

        // Limpar campos
        NovoNome = "";
        NovoAlimento = "";
        NovoTempo = "";
        NovaPotencia = "";
        NovoCaracter = "";

        Instrucoes = $"Programa '{novoAquecimento.Nome}' salvo com sucesso!";
    }

    // INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
