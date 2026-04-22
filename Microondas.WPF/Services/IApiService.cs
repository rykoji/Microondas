using Microondas.Domain.Entities;

namespace Microondas.WPF.Services;

public interface IApiService
{
    bool EstaAutenticado { get; }
    Task<(bool sucesso, string mensagem)> LoginAsync(string username, string password);
    Task<List<AquecimentoCustomizado>> ObterProgramasCustomizadosAsync();
    Task<(bool sucesso, string mensagem)> SalvarProgramaAsync(AquecimentoCustomizado programa);
    Task<(bool sucesso, string mensagem)> ExcluirProgramaAsync(string nome);
}
