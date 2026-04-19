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
            throw new Exception("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(aquecimento.Alimento))
            throw new Exception("Alimento é obrigatório");

        if (aquecimento.Seconds <= 0)
            throw new Exception("Tempo é obrigatório");

        if (aquecimento.PowerLevel < 1 || aquecimento.PowerLevel > 10)
            throw new Exception("Potência deve ser entre 1 e 10");

        if (aquecimento.CaracterAquecimento == default)
            throw new Exception("Caractere de aquecimento é obrigatório");

        ValidarCaractereUnico(aquecimento.CaracterAquecimento);
    }

    private void ValidarCaractereUnico(char caractere)
    {
        if (_caracteresReservados.Contains(caractere))
            throw new Exception($"Caractere '{caractere}' é reservado");

        if (_programasExistentes.Any(p => p.CaracterAquecimento == caractere))
            throw new Exception($"Caractere '{caractere}' já está em uso");
    }
}