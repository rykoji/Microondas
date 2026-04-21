using Microondas.Domain;

namespace Microondas.Domain;

public class PipocaAquecimento : IAquecimento
{
    public string Nome { get; init; } = "Pipoca";
    public int Seconds { get; init; } = 180;
    public int PowerLevel { get; init; } = 7;
    public string Alimento { get; init; } = "Pipoca (de micro-ondas)";
    public string Instrucoes { get; init; } = "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 1 segundo entre um estouro e outro, interrompa o aquecimento";
    public char CaracterAquecimento { get; init; } = 'P';
    public bool IsCustomize => false;
}
