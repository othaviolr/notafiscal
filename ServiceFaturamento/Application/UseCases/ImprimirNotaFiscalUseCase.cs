using ServiceFaturamento.Domain.Repositories;
using ServiceFaturamento.Infrastructure.Http;

namespace ServiceFaturamento.Application.UseCases;

public class ImprimirNotaFiscalUseCase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly EstoqueApiClient _estoqueApiClient;

    public ImprimirNotaFiscalUseCase(
        INotaFiscalRepository notaFiscalRepository,
        EstoqueApiClient estoqueApiClient)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _estoqueApiClient = estoqueApiClient;
    }

    public async Task<ResultadoImpressao> ExecutarAsync(Guid notaFiscalId)
    {
        try
        {
            var nota = await _notaFiscalRepository.ObterPorIdAsync(notaFiscalId);
            if (nota == null)
            {
                return new ResultadoImpressao
                {
                    Sucesso = false,
                    Mensagem = "Nota fiscal não encontrada"
                };
            }

            if (nota.Status == "Fechada")
            {
                return new ResultadoImpressao
                {
                    Sucesso = false,
                    Mensagem = "Esta nota fiscal já foi impressa e está fechada"
                };
            }

            if (!nota.Itens.Any())
            {
                return new ResultadoImpressao
                {
                    Sucesso = false,
                    Mensagem = "A nota fiscal não possui itens para impressão"
                };
            }

            var itensReserva = nota.Itens.Select(i => new ItemReservaDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList();

            var resultadoReserva = await _estoqueApiClient.ReservarProdutosAsync(notaFiscalId, itensReserva);

            if (!resultadoReserva.Sucesso)
            {
                return new ResultadoImpressao
                {
                    Sucesso = false,
                    Mensagem = $"Falha ao reservar produtos: {resultadoReserva.Mensagem}"
                };
            }

            var resultadoConfirmacao = await _estoqueApiClient.ConfirmarReservaAsync(notaFiscalId);

            if (!resultadoConfirmacao.Sucesso)
            {
                await _estoqueApiClient.CompensarReservaAsync(notaFiscalId);

                return new ResultadoImpressao
                {
                    Sucesso = false,
                    Mensagem = $"Falha ao processar nota fiscal: {resultadoConfirmacao.Mensagem}. A reserva foi compensada e o saldo devolvido ao estoque."
                };
            }

            var notaFechada = await _notaFiscalRepository.FecharNotaAsync(notaFiscalId);

            if (!notaFechada)
            {
                return new ResultadoImpressao
                {
                    Sucesso = false,
                    Mensagem = "Erro ao fechar a nota fiscal. Contate o suporte."
                };
            }

            return new ResultadoImpressao
            {
                Sucesso = true,
                Mensagem = $"Nota fiscal {nota.Numero} impressa com sucesso! Estoque atualizado.",
                NotaFiscal = nota
            };
        }
        catch (Exception ex)
        {
            try
            {
                await _estoqueApiClient.CompensarReservaAsync(notaFiscalId);
            }
            catch
            {
            }

            return new ResultadoImpressao
            {
                Sucesso = false,
                Mensagem = $"Erro inesperado ao imprimir nota fiscal: {ex.Message}"
            };
        }
    }
}

public class ResultadoImpressao
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public Domain.Entities.NotaFiscal? NotaFiscal { get; set; }
}