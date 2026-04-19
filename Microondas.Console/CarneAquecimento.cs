using Microondas.Domain;

namespace Microondas.Console
{
    public class CarneAquecimento : IAquecimento
    {
        public string Nome { get; set; }
        public int Seconds { get; set; } = 840;
        public int PowerLevel { get; set; } = 4;
    }
}
