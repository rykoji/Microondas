using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microondas.Domain;

public interface IAquecimento
{
    public string Nome { get; set; }
    public int Seconds { get; set; }
    public int PowerLevel { get; set; }
    string Alimento { get; set; }
    string Instrucoes { get; set; }
    char CaracterAquecimento { get; set; }
    bool IsCustomize { get; }

}
