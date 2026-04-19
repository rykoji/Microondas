using Microondas.Domain;

namespace Microondas.Console
{
    public class FeijaoAquecimento : IAquecimento
    {
        public string Nome { get; set; } = "Feijão";
        public int Seconds { get; set; } = 480;
        public int PowerLevel { get; set; } = 9;
        public string Alimento { get; set; } = "Feijão congelado";
        public string Instrucoes { get; set; } = "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas";
        public char CaracterAquecimento { get; set; } = 'J';
        public bool IsCustomize => false;
    }
}
