using ServiceEstoque.Domain.Entities;

namespace ServiceEstoque.Domain.Repositories;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task<List<Produto>> ObterTodosAsync();
    Task<Produto> CriarAsync(Produto produto);
    Task<bool> AtualizarAsync(Produto produto);
    Task<bool> ReservarSaldoAsync(Guid produtoId, int quantidade);
    Task<bool> ConfirmarBaixaAsync(Guid produtoId, int quantidade);
    Task<bool> DevolverSaldoAsync(Guid produtoId, int quantidade);
}