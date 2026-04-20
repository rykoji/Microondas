namespace Microondas.Domain
{
    public class AquecimentoCustomizado : IAquecimento
    {
        public string Nome { get; set; } = string.Empty;
        public int Seconds { get; set; } 
        public int PowerLevel { get; set; }
        public string Alimento { get; set; } = string.Empty;
        public string Instrucoes { get; set; }
        public char CaracterAquecimento { get; set; }
        public bool IsCustomize => true;
    }
}
