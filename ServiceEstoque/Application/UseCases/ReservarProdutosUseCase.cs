using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Application.UseCases;

public class ReservarProdutosUseCase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IReservaProdutoRepository _reservaRepository;

    public ReservarProdutosUseCase(
        IProdutoRepository produtoRepository,
        IReservaProdutoRepository reservaRepository)
    {
        _produtoRepository = produtoRepository;
        _reservaRepository = reservaRepository;
    }

    public async Task<ResultadoReserva> ExecutarAsync(Guid notaFiscalId, List<ItemReserva> itens)
    {
        var reservasCriadas = new List<Guid>();

        try
        {
            foreach (var item in itens)
            {
                var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
                if (produto == null)
                {
                    return new ResultadoReserva
                    {
                        Sucesso = false,
                        Mensagem = $"Produto {item.ProdutoId} não encontrado"
                    };
                }

                if (produto.SaldoDisponivel < item.Quantidade)
                {
                    return new ResultadoReserva
                    {
                        Sucesso = false,
                        Mensagem = $"Saldo insuficiente para o produto {produto.Nome}. Disponível: {produto.SaldoDisponivel}, Solicitado: {item.Quantidade}"
                    };
                }

                var reservado = await _produtoRepository.ReservarSaldoAsync(item.ProdutoId, item.Quantidade);
                if (!reservado)
                {
                    return new ResultadoReserva
                    {
                        Sucesso = false,
                        Mensagem = $"Erro ao reservar saldo do produto {produto.Nome}"
                    };
                }

                var reserva = await _reservaRepository.CriarAsync(new Domain.Entities.ReservaProduto
                {
                    ProdutoId = item.ProdutoId,
                    NotaFiscalId = notaFiscalId,
                    Quantidade = item.Quantidade
                });

                reservasCriadas.Add(reserva.Id);
            }

            return new ResultadoReserva
            {
                Sucesso = true,
                Mensagem = "Produtos reservados com sucesso",
                ReservasIds = reservasCriadas
            };
        }
        catch (Exception ex)
        {
            return new ResultadoReserva
            {
                Sucesso = false,
                Mensagem = $"Erro ao processar reserva: {ex.Message}"
            };
        }
    }
}

public class ItemReserva
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class ResultadoReserva
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public List<Guid> ReservasIds { get; set; } = new();
}