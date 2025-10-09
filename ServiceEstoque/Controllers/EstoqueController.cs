using Microsoft.AspNetCore.Mvc;
using ServiceEstoque.Application.Services;
using ServiceEstoque.Application.UseCases;
using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstoqueController : ControllerBase
{
    private readonly EstoqueService _estoqueService;

    public EstoqueController(
        IProdutoRepository produtoRepository,
        IReservaProdutoRepository reservaRepository)
    {
        _estoqueService = new EstoqueService(produtoRepository, reservaRepository);
    }

    [HttpPost("reservar")]
    public async Task<IActionResult> ReservarProdutos([FromBody] ReservarProdutosRequest request)
    {
        try
        {
            var resultado = await _estoqueService.ReservarProdutosAsync(
                request.NotaFiscalId,
                request.Itens.Select(i => new ItemReserva
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            );

            if (!resultado.Sucesso)
                return BadRequest(new { mensagem = resultado.Mensagem });

            return Ok(new
            {
                mensagem = resultado.Mensagem,
                reservasIds = resultado.ReservasIds
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao reservar produtos: {ex.Message}" });
        }
    }

    [HttpPost("confirmar")]
    public async Task<IActionResult> ConfirmarReserva([FromBody] ConfirmarReservaRequest request)
    {
        try
        {
            var resultado = await _estoqueService.ConfirmarReservaAsync(request.NotaFiscalId);

            if (!resultado.Sucesso)
                return StatusCode(500, new { mensagem = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao confirmar reserva: {ex.Message}" });
        }
    }

    [HttpPost("compensar")]
    public async Task<IActionResult> CompensarReserva([FromBody] CompensarReservaRequest request)
    {
        try
        {
            var resultado = await _estoqueService.CompensarReservaAsync(request.NotaFiscalId);

            if (!resultado.Sucesso)
                return StatusCode(500, new { mensagem = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao compensar reserva: {ex.Message}" });
        }
    }
}

public class ReservarProdutosRequest
{
    public Guid NotaFiscalId { get; set; }
    public List<ItemReservaRequest> Itens { get; set; } = new();
}

public class ItemReservaRequest
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

public class ConfirmarReservaRequest
{
    public Guid NotaFiscalId { get; set; }
}

public class CompensarReservaRequest
{
    public Guid NotaFiscalId { get; set; }
}