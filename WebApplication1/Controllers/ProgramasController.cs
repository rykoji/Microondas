using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microondas.Domain.Entities;
using Microondas.Domain.Services;
using Microondas.WebApplication.Exceptions;

namespace Microondas.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgramasController : ControllerBase
{
    private readonly AquecimentoService _aquecimentoService;

    public ProgramasController(AquecimentoService aquecimentoService)
    {
        _aquecimentoService = aquecimentoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var programas = await _aquecimentoService.ObterTodosProgramasAsync();
        return Ok(programas.Select(p => new
        {
            p.Nome,
            p.Alimento,
            p.Seconds,
            p.PowerLevel,
            p.CaracterAquecimento,
            p.Instrucoes,
            p.IsCustomize
        }));
    }

    [HttpGet("{nome}")]
    public async Task<IActionResult> ObterPorNome(string nome)
    {
        var programas = await _aquecimentoService.ObterTodosProgramasAsync();
        var programa = programas.FirstOrDefault(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (programa == null)
            return NotFound(new { Message = $"Programa '{nome}' não encontrado" });

        return Ok(new
        {
            programa.Nome,
            programa.Alimento,
            programa.Seconds,
            programa.PowerLevel,
            programa.CaracterAquecimento,
            programa.Instrucoes,
            programa.IsCustomize
        });
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] AquecimentoCustomizado aquecimento)
    {
        try
        {
            await _aquecimentoService.CadastrarAsync(aquecimento);
            return Created($"/api/programas/{aquecimento.Nome}", new
            {
                aquecimento.Nome,
                aquecimento.Alimento,
                aquecimento.Seconds,
                aquecimento.PowerLevel,
                aquecimento.CaracterAquecimento,
                aquecimento.Instrucoes
            });
        }
        catch (ArgumentException ex)
        {
            throw new BusinessException(ex.Message);
        }
    }

    [HttpDelete("{nome}")]
    public async Task<IActionResult> Remover(string nome)
    {
        var programas = await _aquecimentoService.ObterTodosProgramasAsync();
        var programa = programas.FirstOrDefault(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        if (programa == null)
            return NotFound(new { Message = $"Programa '{nome}' não encontrado" });

        if (!programa.IsCustomize)
            throw new BusinessException("Não é permitido remover programas pré-definidos");

        await _aquecimentoService.RemoverAsync(nome);
        return NoContent();
    }
}
