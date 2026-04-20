using Microondas.Domain.Entities;
using Microondas.Domain.Repositories;

namespace Microondas.Domain.Services;

public class AquecimentoService
{
    private readonly IAquecimentoRepository _repository;
    private readonly List<IAquecimento> _programasPreDefinidos;

    public AquecimentoService(
        IAquecimentoRepository repository,
        IEnumerable<IAquecimento> programasPreDefinidos)
    {
        _repository = repository;
        _programasPreDefinidos = programasPreDefinidos.ToList();
    }

    public async Task<List<IAquecimento>> ObterTodosProgramasAsync()
    {
        var customizados = await _repository.ObterTodosAsync();
        return _programasPreDefinidos
            .Concat(customizados)
            .ToList();
    }

    public async Task CadastrarAsync(AquecimentoCustomizado aquecimento)
    {
        var todos = await ObterTodosProgramasAsync();
        var validator = new AquecimentoValidator(todos);
        validator.Validar(aquecimento);

        await _repository.AdicionarAsync(aquecimento);
    }

    public async Task RemoverAsync(string nome)
    {
        await _repository.RemoverAsync(nome);
    }
}