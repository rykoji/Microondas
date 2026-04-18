Console.WriteLine("Hello, World!");


var microondas = new Microondas.Domain.Microondas();
microondas.AdicionarTempo(5);
microondas.SelecionarPotencia(3);
microondas.OnTick += () => Console.Write(new string('.', microondas.PowerLevel) + " ");
microondas.OnFinished += () => Console.Write("Aquecimento concluido");
await microondas.Start();
