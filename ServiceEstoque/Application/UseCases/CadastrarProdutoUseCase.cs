using ServiceEstoque.Domain.Entities;
using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Application.UseCases;

public class CadastrarProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public CadastrarProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<Produto> ExecutarAsync(string nome, string descricao, decimal preco, int saldoInicial)
    {
        var produto = new Produto
        {
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            SaldoDisponivel = saldoInicial,
            SaldoReservado = 0
        };

        return await _produtoRepository.CriarAsync(produto);
    }
}