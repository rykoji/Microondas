namespace Microondas.WPF.Services;

public interface IApiService
{
    bool EstaAutenticado { get; }
    Task<(bool sucesso, string mensagem)> LoginAsync(string username, string password);
}
