using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Application.UseCases;

public class CompensarReservaUseCase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IReservaProdutoRepository _reservaRepository;

    public CompensarReservaUseCase(
        IProdutoRepository produtoRepository,
        IReservaProdutoRepository reservaRepository)
    {
        _produtoRepository = produtoRepository;
        _reservaRepository = reservaRepository;
    }

    public async Task<ResultadoCompensacao> ExecutarAsync(Guid notaFiscalId)
    {
        try
        {
            var reservas = await _reservaRepository.ObterReservasPendentesAsync(notaFiscalId);

            if (!reservas.Any())
            {
                return new ResultadoCompensacao
                {
                    Sucesso = true,
                    Mensagem = "Nenhuma reserva para compensar"
                };
            }

            foreach (var reserva in reservas)
            {
                await _produtoRepository.DevolverSaldoAsync(reserva.ProdutoId, reserva.Quantidade);

                await _reservaRepository.CancelarReservaAsync(reserva.Id);
            }

            return new ResultadoCompensacao
            {
                Sucesso = true,
                Mensagem = "Reservas compensadas com sucesso. Saldo devolvido ao estoque."
            };
        }
        catch (Exception ex)
        {
            return new ResultadoCompensacao
            {
                Sucesso = false,
                Mensagem = $"Erro ao compensar reserva: {ex.Message}"
            };
        }
    }
}

public class ResultadoCompensacao
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}