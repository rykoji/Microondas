using Microondas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microondas.Console;

public class FrangoAquecimento : IAquecimento
{
    public string Nome { get; set; } = "Frango";
    public int Seconds { get; set; } = 480;
    public int PowerLevel { get; set; } = 7;
    public string Alimento { get; set; } = "Frango (qualquer corte)";
    public string Instrucoes { get; set; } = "Interrompa o processo na metade e vire o conteúdo com parte de baixo para cima para o descongelamento uniforme.";
    public char CaracterAquecimento { get; set; } = 'F';
    public bool IsCustomize => false;
}
