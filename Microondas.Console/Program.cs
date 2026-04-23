var microondas = Microondas.Domain.Microondas.Criar();
microondas.OnTick += () => {
    //Console.Write(new string('.', microondas.PowerLevel) + " ");
    Console.WriteLine(microondas.EstaAquecendo);
};
microondas.OnFinished += () =>
{
    //Console.Write("Aquecimento concluido");
    Console.WriteLine(microondas.EstaAquecendo);
};



microondas.AdicionarTempo(6);
microondas.SelecionarPotencia(3);



microondas.Start();
Thread.Sleep(3000);
microondas.Stop();
Console.WriteLine(microondas.EstaAquecendo);


await microondas.Start();


