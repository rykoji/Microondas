namespace Microondas.Domain.Entities
{
    public class AquecimentoCustomizado : IAquecimento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Seconds { get; set; } 
        public int PowerLevel { get; set; }
        public string Alimento { get; set; } = string.Empty;
        public string Instrucoes { get; set; }
        public char CaracterAquecimento { get; set; }
        public bool IsCustomize => true;
    }
}
