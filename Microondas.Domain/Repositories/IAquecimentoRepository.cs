using Microondas.Domain.Entities;

namespace Microondas.Domain.Repositories;

public interface IAquecimentoRepository
{
    Task<List<AquecimentoCustomizado>> ObterTodosAsync();
    Task<AquecimentoCustomizado?> ObterPorNomeAsync(string nome);
    Task AdicionarAsync(AquecimentoCustomizado aquecimento);
    Task AtualizarAsync(AquecimentoCustomizado aquecimento);
    Task RemoverAsync(string nome);
}