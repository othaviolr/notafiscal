using ServiceFaturamento.Domain.Entities;

namespace ServiceFaturamento.Domain.Repositories;

public interface INotaFiscalRepository
{
    Task<NotaFiscal?> ObterPorIdAsync(Guid id);
    Task<List<NotaFiscal>> ObterTodosAsync();
    Task<NotaFiscal> CriarAsync(NotaFiscal notaFiscal);
    Task<bool> AtualizarAsync(NotaFiscal notaFiscal);
    Task<ItemNotaFiscal> AdicionarItemAsync(ItemNotaFiscal item);
    Task<bool> FecharNotaAsync(Guid notaFiscalId);
}