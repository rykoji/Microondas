using Microondas.Domain;

namespace Microondas.Console
{
    public class FeijaoAquecimento : IAquecimento
    {
        public string Nome { get; init; } = "Feijao";
        public int Seconds { get; init; } = 480;
        public int PowerLevel { get; init; } = 9;
        public string Alimento { get; init; } = "Feijão congelado";
        public string Instrucoes { get; init; } = "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas";
        public char CaracterAquecimento { get; init; } = 'J';
        public bool IsCustomize => false;
    }
}
