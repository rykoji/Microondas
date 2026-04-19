using Microondas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microondas.Console;

public class LeiteAquecimento : IAquecimento
{
    public string Nome { get; set; } = "Leite";
    public int Seconds { get; set; } = 300;
    public int PowerLevel { get; set; } = 5;
    public string Alimento { get; set; } = "Leite";
    public string Instrucoes { get; set; } = "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.";
    public char CaracterAquecimento { get; set; } = 'L';
    public bool IsCustomize => false;
}
