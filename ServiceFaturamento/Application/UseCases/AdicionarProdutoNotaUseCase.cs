using ServiceFaturamento.Domain.Entities;
using ServiceFaturamento.Domain.Repositories;
using ServiceFaturamento.Infrastructure.Http;

namespace ServiceFaturamento.Application.UseCases;

public class AdicionarProdutoNotaUseCase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly EstoqueApiClient _estoqueApiClient;

    public AdicionarProdutoNotaUseCase(
        INotaFiscalRepository notaFiscalRepository,
        EstoqueApiClient estoqueApiClient)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _estoqueApiClient = estoqueApiClient;
    }

    public async Task<ResultadoAdicionarProduto> ExecutarAsync(Guid notaFiscalId, Guid produtoId, int quantidade)
    {
        try
        {
            var nota = await _notaFiscalRepository.ObterPorIdAsync(notaFiscalId);
            if (nota == null)
            {
                return new ResultadoAdicionarProduto
                {
                    Sucesso = false,
                    Mensagem = "Nota fiscal não encontrada"
                };
            }

            if (nota.Status == "Fechada")
            {
                return new ResultadoAdicionarProduto
                {
                    Sucesso = false,
                    Mensagem = "Não é possível adicionar produtos em uma nota fiscal fechada"
                };
            }

            var produto = await _estoqueApiClient.ObterProdutoAsync(produtoId);
            if (produto == null)
            {
                return new ResultadoAdicionarProduto
                {
                    Sucesso = false,
                    Mensagem = "Produto não encontrado no estoque"
                };
            }

            var item = new ItemNotaFiscal
            {
                NotaFiscalId = notaFiscalId,
                ProdutoId = produtoId,
                NomeProduto = produto.Nome,
                Quantidade = quantidade,
                PrecoUnitario = produto.Preco
            };

            await _notaFiscalRepository.AdicionarItemAsync(item);

            return new ResultadoAdicionarProduto
            {
                Sucesso = true,
                Mensagem = "Produto adicionado à nota fiscal com sucesso",
                Item = item
            };
        }
        catch (Exception ex)
        {
            return new ResultadoAdicionarProduto
            {
                Sucesso = false,
                Mensagem = $"Erro ao adicionar produto: {ex.Message}"
            };
        }
    }
}

public class ResultadoAdicionarProduto
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public ItemNotaFiscal? Item { get; set; }
}