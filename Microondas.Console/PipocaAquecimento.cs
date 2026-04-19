using Microondas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microondas.Console;

public class PipocaAquecimento : IAquecimento
{
    public string Nome { get; set; }
    public int Seconds { get; set; } = 180;
    public int PowerLevel { get; set; } = 7;
}
