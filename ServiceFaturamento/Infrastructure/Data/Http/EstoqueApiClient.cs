using System.Text;
using System.Text.Json;

namespace ServiceFaturamento.Infrastructure.Http;

public class EstoqueApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public EstoqueApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["EstoqueApi:BaseUrl"] ?? "http://service-estoque:5001";
    }

    public async Task<RespostaReserva> ReservarProdutosAsync(Guid notaFiscalId, List<ItemReservaDto> itens)
    {
        try
        {
            var request = new
            {
                notaFiscalId = notaFiscalId,
                itens = itens
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/estoque/reservar", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var erroObj = JsonSerializer.Deserialize<RespostaErro>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new RespostaReserva
                {
                    Sucesso = false,
                    Mensagem = erroObj?.Mensagem ?? $"Erro HTTP {response.StatusCode}: {responseContent}"
                };
            }

            var resultado = JsonSerializer.Deserialize<RespostaReservaApi>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new RespostaReserva
            {
                Sucesso = true,
                Mensagem = resultado?.Mensagem ?? "Produtos reservados com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new RespostaReserva
            {
                Sucesso = false,
                Mensagem = $"Erro ao comunicar com serviço de estoque: {ex.Message}"
            };
        }
    }

    public async Task<RespostaConfirmacao> ConfirmarReservaAsync(Guid notaFiscalId)
    {
        try
        {
            var request = new { notaFiscalId = notaFiscalId };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/estoque/confirmar", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var erroObj = JsonSerializer.Deserialize<RespostaErro>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new RespostaConfirmacao
                {
                    Sucesso = false,
                    Mensagem = erroObj?.Mensagem ?? $"Erro HTTP {response.StatusCode}: {responseContent}"
                };
            }

            var resultado = JsonSerializer.Deserialize<RespostaConfirmacaoApi>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new RespostaConfirmacao
            {
                Sucesso = true,
                Mensagem = resultado?.Mensagem ?? "Reserva confirmada com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new RespostaConfirmacao
            {
                Sucesso = false,
                Mensagem = $"Erro ao comunicar com serviço de estoque: {ex.Message}"
            };
        }
    }

    public async Task<RespostaCompensacao> CompensarReservaAsync(Guid notaFiscalId)
    {
        try
        {
            var request = new { notaFiscalId = notaFiscalId };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/estoque/compensar", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var erroObj = JsonSerializer.Deserialize<RespostaErro>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new RespostaCompensacao
                {
                    Sucesso = false,
                    Mensagem = erroObj?.Mensagem ?? $"Erro HTTP {response.StatusCode}"
                };
            }

            var resultado = JsonSerializer.Deserialize<RespostaCompensacaoApi>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new RespostaCompensacao
            {
                Sucesso = true,
                Mensagem = resultado?.Mensagem ?? "Reserva compensada com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new RespostaCompensacao
            {
                Sucesso = false,
                Mensagem = $"Erro ao compensar reserva: {ex.Message}"
            };
        }
    }

    public async Task<ProdutoDto?> ObterProdutoAsync(Guid produtoId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/produtos/{produtoId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var produto = JsonSerializer.Deserialize<ProdutoDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return produto;
        }
        catch
        {
            return null;
        }
    }
}

// Classes auxiliares para deserialização
public class RespostaReservaApi
{
    public string Mensagem { get; set; } = string.Empty;
    public List<Guid> ReservasIds { get; set; } = new();
}

public class RespostaConfirmacaoApi
{
    public string Mensagem { get; set; } = string.Empty;
}

public class RespostaCompensacaoApi
{
    public string Mensagem { get; set; } = string.Empty;
}

public class RespostaErro
{
    public string Mensagem { get; set; } = string.Empty;
}

// DTOs públicos
public class ItemReservaDto
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class RespostaReserva
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}

public class RespostaConfirmacao
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}

public class RespostaCompensacao
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}

public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int SaldoDisponivel { get; set; }
}