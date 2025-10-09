using Microsoft.EntityFrameworkCore;
using ServiceFaturamento.Domain.Entities;
using ServiceFaturamento.Domain.Repositories;

namespace ServiceFaturamento.Infrastructure.Data.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public NotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context;
    }

    public async Task<NotaFiscal?> ObterPorIdAsync(Guid id)
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<List<NotaFiscal>> ObterTodosAsync()
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .ToListAsync();
    }

    public async Task<NotaFiscal> CriarAsync(NotaFiscal notaFiscal)
    {
        notaFiscal.Id = Guid.NewGuid();
        notaFiscal.DataEmissao = DateTime.UtcNow;
        notaFiscal.Status = "Aberta";
        notaFiscal.Version = 1;
        notaFiscal.ValorTotal = 0;

        await _context.NotasFiscais.AddAsync(notaFiscal);
        await _context.SaveChangesAsync();

        return notaFiscal;
    }

    public async Task<bool> AtualizarAsync(NotaFiscal notaFiscal)
    {
        try
        {
            _context.NotasFiscais.Update(notaFiscal);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task<ItemNotaFiscal> AdicionarItemAsync(ItemNotaFiscal item)
    {
        item.Id = Guid.NewGuid();

        await _context.ItensNotaFiscal.AddAsync(item);
        await _context.SaveChangesAsync();

        var nota = await ObterPorIdAsync(item.NotaFiscalId);
        if (nota != null)
        {
            nota.ValorTotal = nota.Itens.Sum(i => i.ValorTotal);
            nota.Version++;
            await AtualizarAsync(nota);
        }

        return item;
    }

    public async Task<bool> FecharNotaAsync(Guid notaFiscalId)
    {
        var nota = await ObterPorIdAsync(notaFiscalId);

        if (nota == null || nota.Status == "Fechada")
            return false;

        nota.Status = "Fechada";
        nota.DataFechamento = DateTime.UtcNow;
        nota.Version++;

        return await AtualizarAsync(nota);
    }
}