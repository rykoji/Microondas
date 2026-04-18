Console.WriteLine("Hello, World!");


var microondas = new Microondas.Domain.Microondas();
microondas.AdicionarTempo(1);
microondas.OnTick += () => Console.WriteLine("aaa");
microondas.OnFinished += () => Console.WriteLine("bbb");
await microondas.Start();

Console.WriteLine(microondas.PowerLevel);