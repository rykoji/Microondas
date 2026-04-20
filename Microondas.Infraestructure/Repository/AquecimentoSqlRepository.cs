using Microsoft.EntityFrameworkCore;
using Microondas.Domain;
using Microondas.Domain.Repositories;
using Microondas.Infrastructure.Data;
using Microondas.Domain.Entities;

namespace Microondas.Infrastructure.Repositories;

public class AquecimentoSqlRepository : IAquecimentoRepository
{
    private readonly MicroondasDbContext _context;

    public AquecimentoSqlRepository(MicroondasDbContext context)
    {
        _context = context;
    }

    public async Task<List<AquecimentoCustomizado>> ObterTodosAsync()
    {
        return await _context.ProgramasCustomizados.ToListAsync();
    }

    public async Task<AquecimentoCustomizado?> ObterPorNomeAsync(string nome)
    {
        return await _context.ProgramasCustomizados
            .FirstOrDefaultAsync(p => p.Nome == nome);
    }

    public async Task AdicionarAsync(AquecimentoCustomizado aquecimento)
    {
        await _context.ProgramasCustomizados.AddAsync(aquecimento);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(AquecimentoCustomizado aquecimento)
    {
        _context.ProgramasCustomizados.Update(aquecimento);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(string nome)
    {
        var programa = await ObterPorNomeAsync(nome);
        if (programa != null)
        {
            _context.ProgramasCustomizados.Remove(programa);
            await _context.SaveChangesAsync();
        }
    }
}