using Microondas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microondas.Console;

public class PipocaAquecimento : IAquecimento
{
    public string Nome { get; set; } = "Pipoca";
    public int Seconds { get; set; } = 180;
    public int PowerLevel { get; set; } = 7;
    public string Alimento { get; set; } = "Pipoca (de micro-ondas)";
    public string Instrucoes { get; set; } = "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 1 segundo entre um estouro e outro, interrompa o aquecimento";
    public char CaracterAquecimento { get; set; } = 'P';
    public bool IsCustomize => false;
}
