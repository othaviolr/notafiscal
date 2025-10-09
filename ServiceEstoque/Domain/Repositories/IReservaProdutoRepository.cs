using ServiceEstoque.Domain.Entities;

namespace ServiceEstoque.Domain.Repositories;

public interface IReservaProdutoRepository
{
    Task<ReservaProduto> CriarAsync(ReservaProduto reserva);
    Task<List<ReservaProduto>> ObterPorNotaFiscalAsync(Guid notaFiscalId);
    Task<bool> ConfirmarReservaAsync(Guid reservaId);
    Task<bool> CancelarReservaAsync(Guid reservaId);
    Task<List<ReservaProduto>> ObterReservasPendentesAsync(Guid notaFiscalId);
}