using Microondas.Domain;

namespace Microondas.Console
{
    public class CarneAquecimento : IAquecimento
    {
        public string Nome { get; set; } = "Carnes de boi";
        public int Seconds { get; set; } = 840;
        public int PowerLevel { get; set; } = 4;
        public string Alimento { get; set; } = "Carne em pedaço ou fatias";
        public string Instrucoes { get; set; } = "Interrompa o processo na metade e vire o conteúdo com parte de baixo para cima para o descongelamento uniforme.";
        public char CaracterAquecimento { get; set; } = 'C';
        public bool IsCustomize => false;
    }
}
