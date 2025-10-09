using Microsoft.EntityFrameworkCore;
using ServiceEstoque.Domain.Entities;
using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Infrastructure.Data.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ProdutoRepository(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<List<Produto>> ObterTodosAsync()
    {
        return await _context.Produtos.ToListAsync();
    }

    public async Task<Produto> CriarAsync(Produto produto)
    {
        produto.Id = Guid.NewGuid();
        produto.DataCriacao = DateTime.UtcNow;
        produto.DataAtualizacao = DateTime.UtcNow;
        produto.Version = 1;

        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();

        return produto;
    }

    public async Task<bool> AtualizarAsync(Produto produto)
    {
        try
        {
            produto.DataAtualizacao = DateTime.UtcNow;
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task<bool> ReservarSaldoAsync(Guid produtoId, int quantidade)
    {
        var produto = await ObterPorIdAsync(produtoId);

        if (produto == null || produto.SaldoDisponivel < quantidade)
            return false;

        produto.SaldoDisponivel -= quantidade;
        produto.SaldoReservado += quantidade;
        produto.Version++;

        return await AtualizarAsync(produto);
    }

    public async Task<bool> ConfirmarBaixaAsync(Guid produtoId, int quantidade)
    {
        var produto = await ObterPorIdAsync(produtoId);

        if (produto == null || produto.SaldoReservado < quantidade)
            return false;

        produto.SaldoReservado -= quantidade;
        produto.Version++;

        return await AtualizarAsync(produto);
    }

    public async Task<bool> DevolverSaldoAsync(Guid produtoId, int quantidade)
    {
        var produto = await ObterPorIdAsync(produtoId);

        if (produto == null)
            return false;

        produto.SaldoReservado -= quantidade;
        produto.SaldoDisponivel += quantidade;
        produto.Version++;

        return await AtualizarAsync(produto);
    }
}