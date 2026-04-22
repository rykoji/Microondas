using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Microondas.WPF.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    public ApiService(string baseUrl = "http://localhost:5094")
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _httpClient = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
    }

    public bool EstaAutenticado => !string.IsNullOrEmpty(_token);

    public async Task<(bool sucesso, string mensagem)> LoginAsync(string username, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                Username = username,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
                return (false, "Credenciais inválidas.");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            _token = json.RootElement.GetProperty("token").GetString();

            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                return (true, "Autenticado.");
            }

            return (false, "Token não recebido.");
        }
        catch (Exception ex)
        {
            _token = null;
            return (false, $"Erro: {ex.Message}");
        }
    }
}
