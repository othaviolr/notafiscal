using Microsoft.EntityFrameworkCore;
using ServiceEstoque.Domain.Entities;
using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Infrastructure.Data.Repositories;

public class ReservaProdutoRepository : IReservaProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ReservaProdutoRepository(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task<ReservaProduto> CriarAsync(ReservaProduto reserva)
    {
        reserva.Id = Guid.NewGuid();
        reserva.DataReserva = DateTime.UtcNow;
        reserva.Status = "Pendente";

        await _context.ReservasProdutos.AddAsync(reserva);
        await _context.SaveChangesAsync();

        return reserva;
    }

    public async Task<List<ReservaProduto>> ObterPorNotaFiscalAsync(Guid notaFiscalId)
    {
        return await _context.ReservasProdutos
            .Include(r => r.Produto)
            .Where(r => r.NotaFiscalId == notaFiscalId)
            .ToListAsync();
    }

    public async Task<bool> ConfirmarReservaAsync(Guid reservaId)
    {
        var reserva = await _context.ReservasProdutos.FindAsync(reservaId);

        if (reserva == null || reserva.Status != "Pendente")
            return false;

        reserva.Status = "Confirmada";
        reserva.DataConfirmacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelarReservaAsync(Guid reservaId)
    {
        var reserva = await _context.ReservasProdutos.FindAsync(reservaId);

        if (reserva == null)
            return false;

        reserva.Status = "Cancelada";
        reserva.DataConfirmacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ReservaProduto>> ObterReservasPendentesAsync(Guid notaFiscalId)
    {
        return await _context.ReservasProdutos
            .Include(r => r.Produto)
            .Where(r => r.NotaFiscalId == notaFiscalId && r.Status == "Pendente")
            .ToListAsync();
    }
}