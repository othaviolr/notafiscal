using ServiceEstoque.Application.UseCases;
using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Application.Services;

public class EstoqueService
{
    private readonly CadastrarProdutoUseCase _cadastrarProdutoUseCase;
    private readonly ReservarProdutosUseCase _reservarProdutosUseCase;
    private readonly ConfirmarReservaUseCase _confirmarReservaUseCase;
    private readonly CompensarReservaUseCase _compensarReservaUseCase;

    public EstoqueService(
        IProdutoRepository produtoRepository,
        IReservaProdutoRepository reservaRepository)
    {
        _cadastrarProdutoUseCase = new CadastrarProdutoUseCase(produtoRepository);
        _reservarProdutosUseCase = new ReservarProdutosUseCase(produtoRepository, reservaRepository);
        _confirmarReservaUseCase = new ConfirmarReservaUseCase(produtoRepository, reservaRepository);
        _compensarReservaUseCase = new CompensarReservaUseCase(produtoRepository, reservaRepository);
    }

    public Task<Domain.Entities.Produto> CadastrarProdutoAsync(string nome, string descricao, decimal preco, int saldoInicial)
        => _cadastrarProdutoUseCase.ExecutarAsync(nome, descricao, preco, saldoInicial);

    public Task<ResultadoReserva> ReservarProdutosAsync(Guid notaFiscalId, List<ItemReserva> itens)
        => _reservarProdutosUseCase.ExecutarAsync(notaFiscalId, itens);

    public Task<ResultadoConfirmacao> ConfirmarReservaAsync(Guid notaFiscalId)
        => _confirmarReservaUseCase.ExecutarAsync(notaFiscalId);

    public Task<ResultadoCompensacao> CompensarReservaAsync(Guid notaFiscalId)
        => _compensarReservaUseCase.ExecutarAsync(notaFiscalId);
}