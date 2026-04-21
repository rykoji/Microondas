using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microondas.Domain;
using Microondas.Domain.Entities;
using Microondas.Console;
using Microondas.Domain.Exceptions;

namespace Microondas.WPF.ViewModels;

public class MicroondasViewModel : INotifyPropertyChanged
{
    private readonly Domain.Microondas _microondas;
    private readonly List<IAquecimento> _programas;
    private ObservableCollection<IAquecimento> _programasCustomizados = new();

    private string _tempoInputText = "";
    private string _potenciaInputText = "";
    private string _campoAtivo = "tempo";
    private string _animacaoTexto = "";
    private string _instrucoes = "Selecione um programa ou configure manualmente.";

    private string _novoNome = "";
    private string _novoAlimento = "";
    private string _novoTempo = "";
    private string _novaPotencia = "";
    private string _novoCaracter = "";

    public MicroondasViewModel()
    {
        _microondas = Domain.Microondas.Criar();

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
                AnimacaoTexto = "Aquecimento concluído!!";
                Instrucoes = "Aquecimento concluído!";
            });
        };

        IniciarCommand = new RelayCommand(Iniciar);
        PararCommand = new RelayCommand(Parar);
        SelecionarProgramaCommand = new RelayCommand<string>(SelecionarPrograma);
        SalvarCustomizadoCommand = new RelayCommand(SalvarCustomizado);
        TeclaNumericaCommand = new RelayCommand<string>(TeclaNumericaPressionada);
        SelecionarCampoCommand = new RelayCommand<string>(SelecionarCampo);
        LimparCampoCommand = new RelayCommand(LimparCampo);
        ExcluirCustomizadoCommand = new RelayCommand<string>(ExcluirCustomizado);
    }

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

    public string TempoInputText
    {
        get => _tempoInputText;
        set { _tempoInputText = value; OnPropertyChanged(); }
    }

    public string PotenciaInputText
    {
        get => _potenciaInputText;
        set { _potenciaInputText = value; OnPropertyChanged(); }
    }

    public string CampoAtivo
    {
        get => _campoAtivo;
        set { _campoAtivo = value; OnPropertyChanged(); }
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

    public ObservableCollection<IAquecimento> ProgramasCustomizados
    {
        get => _programasCustomizados;
        set { _programasCustomizados = value; OnPropertyChanged(); }
    }

    public ICommand IniciarCommand { get; }
    public ICommand PararCommand { get; }
    public ICommand SelecionarProgramaCommand { get; }
    public ICommand SalvarCustomizadoCommand { get; }
    public ICommand TeclaNumericaCommand { get; }
    public ICommand SelecionarCampoCommand { get; }
    public ICommand LimparCampoCommand { get; }
    public ICommand ExcluirCustomizadoCommand { get; }

    private bool _estaPausado = false;

    private void TeclaNumericaPressionada(string? tecla)
    {
        if (tecla == null) return;

        if (_campoAtivo == "tempo")
        {
            if (_tempoInputText.Length < 3)
                TempoInputText += tecla;
        }
        else
        {
            if (_potenciaInputText.Length < 2)
                PotenciaInputText += tecla;
        }
    }

    private void SelecionarCampo(string? campo)
    {
        if (campo == null) return;
        
        if (campo == "alternar" || campo == CampoAtivo)
        {
            CampoAtivo = CampoAtivo == "tempo" ? "potencia" : "tempo";
        }
        else
        {
            CampoAtivo = campo;
        }
    }

    private void LimparCampo()
    {
        if (_campoAtivo == "tempo")
            TempoInputText = "";
        else
            PotenciaInputText = "";
    }

    private int GetTempoInput()
    {
        if (string.IsNullOrEmpty(_tempoInputText))
            return 30;
        return int.TryParse(_tempoInputText, out int tempo) ? tempo : 30;
    }

    private int GetPotenciaInput()
    {
        if (string.IsNullOrEmpty(_potenciaInputText))
            return 10;
        return int.TryParse(_potenciaInputText, out int potencia) ? potencia : 10;
    }

    private async void Iniciar()
    {
        try
        {
            if (!_microondas.EstaAquecendo)
            {
                if (!_estaPausado)
                {
                    _microondas.AdicionarTempo(GetTempoInput());
                    _microondas.SelecionarPotencia(GetPotenciaInput());
                    _microondas.ResetarCaracterAquecimento();
                }
                _estaPausado = false;
            }
            
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
        if (_microondas.EstaAquecendo)
        {
            _estaPausado = true;
        }
        else
        {
            _estaPausado = false;
        }
        
        _microondas.Stop();
        AnimacaoTexto = "";
        Instrucoes = _estaPausado ? "Aquecimento pausado." : "Aquecimento interrompido.";
        OnPropertyChanged(nameof(TempoFormatado));
    }

    private async void SelecionarPrograma(string? nome)
    {
        if (nome is null) return;

        var programa = _programas.FirstOrDefault(p => p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
        if (programa is null) return;

        if (_microondas.usandoProgramaPreDefinido)
        {
            Instrucoes = "Não é permitido acrescentar tempo em programas pré - definidos";
            return;
        }
        ;

        Instrucoes = programa.Instrucoes;
        try
        {
            await _microondas.StartWithAquecimento(programa);
        }
        catch (DomainException e)
        {
            Instrucoes = e.Message;
        }

    }
    private void AtualizarAnimacao()
    {
        if (_microondas.EstaAquecendo)
        {
            var caracteres = new string(_microondas.CaracterAquecimento, _microondas.PowerLevel * 3);
            AnimacaoTexto = $"~ {caracteres} ~";
        }
    }

    private void SalvarCustomizado()
    {
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

        var caracterExistente = _programas.Any(p => p.CaracterAquecimento == NovoCaracter[0]);
        if (caracterExistente)
        {
            Instrucoes = "Erro: Este caracter já está em uso por outro programa.";
            return;
        }

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
        ProgramasCustomizados.Add(novoAquecimento);

        NovoNome = "";
        NovoAlimento = "";
        NovoTempo = "";
        NovaPotencia = "";
        NovoCaracter = "";

        Instrucoes = $"Programa '{novoAquecimento.Nome}' salvo com sucesso!";
    }

    private void ExcluirCustomizado(string? nome)
    {
        if (nome == null) return;

        var programa = ProgramasCustomizados.FirstOrDefault(p => p.Nome == nome);
        if (programa != null)
        {
            ProgramasCustomizados.Remove(programa);
            _programas.Remove(programa);
            Instrucoes = $"Programa '{nome}' excluído com sucesso!";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
