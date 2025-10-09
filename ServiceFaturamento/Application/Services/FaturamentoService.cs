using ServiceFaturamento.Application.UseCases;
using ServiceFaturamento.Domain.Repositories;
using ServiceFaturamento.Infrastructure.Http;

namespace ServiceFaturamento.Application.Services;

public class FaturamentoService
{
    private readonly CadastrarNotaFiscalUseCase _cadastrarNotaFiscalUseCase;
    private readonly AdicionarProdutoNotaUseCase _adicionarProdutoNotaUseCase;
    private readonly ImprimirNotaFiscalUseCase _imprimirNotaFiscalUseCase;

    public FaturamentoService(
        INotaFiscalRepository notaFiscalRepository,
        EstoqueApiClient estoqueApiClient)
    {
        _cadastrarNotaFiscalUseCase = new CadastrarNotaFiscalUseCase(notaFiscalRepository);
        _adicionarProdutoNotaUseCase = new AdicionarProdutoNotaUseCase(notaFiscalRepository, estoqueApiClient);
        _imprimirNotaFiscalUseCase = new ImprimirNotaFiscalUseCase(notaFiscalRepository, estoqueApiClient);
    }

    public Task<Domain.Entities.NotaFiscal> CadastrarNotaFiscalAsync(string numero)
        => _cadastrarNotaFiscalUseCase.ExecutarAsync(numero);

    public Task<ResultadoAdicionarProduto> AdicionarProdutoAsync(Guid notaFiscalId, Guid produtoId, int quantidade)
        => _adicionarProdutoNotaUseCase.ExecutarAsync(notaFiscalId, produtoId, quantidade);

    public Task<ResultadoImpressao> ImprimirNotaFiscalAsync(Guid notaFiscalId)
        => _imprimirNotaFiscalUseCase.ExecutarAsync(notaFiscalId);
}