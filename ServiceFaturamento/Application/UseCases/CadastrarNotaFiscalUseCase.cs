using ServiceFaturamento.Domain.Entities;
using ServiceFaturamento.Domain.Repositories;

namespace ServiceFaturamento.Application.UseCases;

public class CadastrarNotaFiscalUseCase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;

    public CadastrarNotaFiscalUseCase(INotaFiscalRepository notaFiscalRepository)
    {
        _notaFiscalRepository = notaFiscalRepository;
    }

    public async Task<NotaFiscal> ExecutarAsync(string numero)
    {
        var notaFiscal = new NotaFiscal
        {
            Numero = numero
        };

        return await _notaFiscalRepository.CriarAsync(notaFiscal);
    }
}