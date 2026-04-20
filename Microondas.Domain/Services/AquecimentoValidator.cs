using Microondas.Domain.Entities;

namespace Microondas.Domain.Services;

public class AquecimentoValidator
{
    private readonly HashSet<char> _caracteresReservados = new() { '.' };
    private readonly List<IAquecimento> _programasExistentes;

    public AquecimentoValidator(IEnumerable<IAquecimento> programasExistentes)
    {
        _programasExistentes = programasExistentes.ToList();
    }

    public void Validar(AquecimentoCustomizado aquecimento)
    {
        if (string.IsNullOrWhiteSpace(aquecimento.Nome))
            throw new ArgumentException ("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(aquecimento.Alimento))
            throw new ArgumentException ("Alimento é obrigatório");

        if (aquecimento.Seconds <= 0)
            throw new ArgumentException ("Tempo é obrigatório");

        if (aquecimento.PowerLevel < 1 || aquecimento.PowerLevel > 10)
            throw new ArgumentException ("Potência deve ser entre 1 e 10");

        if (aquecimento.CaracterAquecimento == default)
            throw new ArgumentException ("Caractere de aquecimento é obrigatório");

        ValidarCaractereUnico(aquecimento.CaracterAquecimento);
    }

    private void ValidarCaractereUnico(char caractere)
    {
        if (_caracteresReservados.Contains(caractere))
            throw new ArgumentException($"Caractere '{caractere}' é reservado");

        if (_programasExistentes.Any(p => p.CaracterAquecimento == caractere))
            throw new ArgumentException($"Caractere '{caractere}' já está em uso");
    }
}