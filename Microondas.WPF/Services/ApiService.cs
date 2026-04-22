using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microondas.Domain.Entities;

namespace Microondas.WPF.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

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

    public async Task<List<AquecimentoCustomizado>> ObterProgramasCustomizadosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/programas");
            if (!response.IsSuccessStatusCode) return [];

            var content = await response.Content.ReadAsStringAsync();
            var todos = JsonSerializer.Deserialize<List<ProgramaDto>>(content, _jsonOptions) ?? [];
            return todos
                .Where(p => p.IsCustomize)
                .Select(p => new AquecimentoCustomizado
                {
                    Nome = p.Nome,
                    Alimento = p.Alimento,
                    Seconds = p.Seconds,
                    PowerLevel = p.PowerLevel,
                    CaracterAquecimento = p.CaracterAquecimento,
                    Instrucoes = p.Instrucoes
                }).ToList();
        }
        catch
        {
            return [];
        }
    }

    public async Task<(bool sucesso, string mensagem)> SalvarProgramaAsync(AquecimentoCustomizado programa)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/programas", new
            {
                programa.Nome,
                programa.Alimento,
                programa.Seconds,
                programa.PowerLevel,
                programa.CaracterAquecimento,
                programa.Instrucoes
            });

            return response.IsSuccessStatusCode
                ? (true, "Salvo com sucesso.")
                : (false, $"Erro ao salvar: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return (false, $"Erro: {ex.Message}");
        }
    }

    public async Task<(bool sucesso, string mensagem)> ExcluirProgramaAsync(string nome)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/programas/{Uri.EscapeDataString(nome)}");
            return response.IsSuccessStatusCode
                ? (true, "Excluído com sucesso.")
                : (false, $"Erro ao excluir: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return (false, $"Erro: {ex.Message}");
        }
    }

    private class ProgramaDto
    {
        public string Nome { get; set; } = "";
        public string Alimento { get; set; } = "";
        public int Seconds { get; set; }
        public int PowerLevel { get; set; }
        public char CaracterAquecimento { get; set; }
        public string Instrucoes { get; set; } = "";
        public bool IsCustomize { get; set; }
    }
}
