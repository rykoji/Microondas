using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microondas.Domain;

public interface IAquecimento
{
    public string Nome { get; }
    public int Seconds { get; }
    public int PowerLevel { get; }
    string Alimento { get; }
    string Instrucoes { get; }
    char CaracterAquecimento { get; }
    bool IsCustomize { get; }

}
