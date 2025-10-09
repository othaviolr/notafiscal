using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Application.UseCases;

public class ConfirmarReservaUseCase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IReservaProdutoRepository _reservaRepository;
    private static readonly Random _random = new Random();

    public ConfirmarReservaUseCase(
        IProdutoRepository produtoRepository,
        IReservaProdutoRepository reservaRepository)
    {
        _produtoRepository = produtoRepository;
        _reservaRepository = reservaRepository;
    }

    public async Task<ResultadoConfirmacao> ExecutarAsync(Guid notaFiscalId)
    {
        if (_random.Next(0, 2) == 0)
        {
            return new ResultadoConfirmacao
            {
                Sucesso = false,
                Mensagem = "Falha simulada no serviço de estoque ao confirmar reserva. Tente novamente."
            };
        }

        try
        {
            var reservas = await _reservaRepository.ObterReservasPendentesAsync(notaFiscalId);

            if (!reservas.Any())
            {
                return new ResultadoConfirmacao
                {
                    Sucesso = false,
                    Mensagem = "Nenhuma reserva pendente encontrada para esta nota fiscal"
                };
            }

            foreach (var reserva in reservas)
            {
                var confirmado = await _produtoRepository.ConfirmarBaixaAsync(reserva.ProdutoId, reserva.Quantidade);

                if (!confirmado)
                {
                    return new ResultadoConfirmacao
                    {
                        Sucesso = false,
                        Mensagem = $"Erro ao confirmar baixa do produto {reserva.ProdutoId}"
                    };
                }

                await _reservaRepository.ConfirmarReservaAsync(reserva.Id);
            }

            return new ResultadoConfirmacao
            {
                Sucesso = true,
                Mensagem = "Reservas confirmadas e baixa realizada com sucesso"
            };
        }
        catch (Exception ex)
        {
            return new ResultadoConfirmacao
            {
                Sucesso = false,
                Mensagem = $"Erro ao confirmar reserva: {ex.Message}"
            };
        }
    }
}

public class ResultadoConfirmacao
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}