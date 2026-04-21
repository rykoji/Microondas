using Microondas.Domain;

namespace Microondas.Domain;

public class LeiteAquecimento : IAquecimento
{
    public string Nome { get; init; } = "Leite";
    public int Seconds { get; init; } = 300;
    public int PowerLevel { get; init; } = 5;
    public string Alimento { get; init; } = "Leite";
    public string Instrucoes { get; init; } = "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.";
    public char CaracterAquecimento { get; init; } = 'L';
    public bool IsCustomize => false;
}
