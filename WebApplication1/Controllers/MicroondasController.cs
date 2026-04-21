using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microondas.WebApplication.Exceptions;

namespace Microondas.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MicroondasController : ControllerBase
{
    private static Domain.Microondas? _microondas;

    [HttpPost("iniciar")]
    public IActionResult Iniciar([FromQuery] int? tempo, [FromQuery] int? potencia)
    {
        _microondas = Domain.Microondas.Criar();

        if (tempo.HasValue)
            _microondas.AdicionarTempo(tempo.Value);

        if (potencia.HasValue)
            _microondas.SelecionarPotencia(potencia.Value);

        _ = _microondas.Start();

        return Ok(new
        {
            Message = "Aquecimento iniciado",
            Tempo = _microondas.Seconds,
            Potencia = _microondas.PowerLevel,
            Caracter = _microondas.CaracterAquecimento
        });
    }

    [HttpPost("parar")]
    public IActionResult Parar()
    {
        if (_microondas == null)
            throw new BusinessException("Microondas não iniciado");

        _microondas.Stop();

        return Ok(new { Message = "Aquecimento parado/cancelado" });
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        if (_microondas == null)
            return Ok(new { Message = "Microondas não iniciado" });

        return Ok(new
        {
            EstaAquecendo = _microondas.EstaAquecendo,
            TempoRestante = _microondas.Seconds,
            Potencia = _microondas.PowerLevel,
            Caracter = _microondas.CaracterAquecimento
        });
    }

    [HttpPost("iniciar-programa/{nomeProgramma}")]
    public IActionResult IniciarComPrograma(string nomeProgramma, [FromServices] Domain.Services.AquecimentoService aquecimentoService)
    {
        var programas = aquecimentoService.ObterTodosProgramasAsync().Result;
        var programa = programas.FirstOrDefault(p => p.Nome.Equals(nomeProgramma, StringComparison.OrdinalIgnoreCase));

        if (programa == null)
            throw new BusinessException($"Programa '{nomeProgramma}' não encontrado");

        _microondas = Domain.Microondas.Criar();
        _ = _microondas.StartWithAquecimento(programa);

        return Ok(new
        {
            Message = $"Aquecimento iniciado com programa '{programa.Nome}'",
            Tempo = _microondas.Seconds,
            Potencia = _microondas.PowerLevel,
            Caracter = _microondas.CaracterAquecimento,
            Instrucoes = programa.Instrucoes
        });
    }
}
