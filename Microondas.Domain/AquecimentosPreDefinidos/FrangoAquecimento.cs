using Microondas.Domain;

namespace Microondas.Domain;

public class FrangoAquecimento : IAquecimento
{
    public string Nome { get; init; } = "Frango";
    public int Seconds { get; init; } = 480;
    public int PowerLevel { get; init; } = 7;
    public string Alimento { get; init; } = "Frango (qualquer corte)";
    public string Instrucoes { get; init; } = "Interrompa o processo na metade e vire o conteúdo com parte de baixo para cima para o descongelamento uniforme.";
    public char CaracterAquecimento { get; init; } = 'F';
    public bool IsCustomize => false;
}
