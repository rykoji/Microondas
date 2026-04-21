using Microondas.Domain;

namespace Microondas.Console
{
    public class CarneAquecimento : IAquecimento
    {
        public string Nome { get; init; } = "Carnes de boi";
        public int Seconds { get; init; } = 840;
        public int PowerLevel { get; init; } = 4;
        public string Alimento { get; init; } = "Carne em pedaço ou fatias";
        public string Instrucoes { get; init; } = "Interrompa o processo na metade e vire o conteúdo com parte de baixo para cima para o descongelamento uniforme.";
        public char CaracterAquecimento { get; init; } = 'C';
        public bool IsCustomize => false;
    }
}
