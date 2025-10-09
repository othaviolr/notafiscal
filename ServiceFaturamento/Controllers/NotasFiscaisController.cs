using Microsoft.AspNetCore.Mvc;
using ServiceFaturamento.Application.Services;
using ServiceFaturamento.Domain.Repositories;
using ServiceFaturamento.Infrastructure.Http;

namespace ServiceFaturamento.Api.Controllers;

[ApiController]
[Route("api/notas-fiscais")]
public class NotasFiscaisController : ControllerBase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly FaturamentoService _faturamentoService;

    public NotasFiscaisController(
        INotaFiscalRepository notaFiscalRepository,
        EstoqueApiClient estoqueApiClient)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _faturamentoService = new FaturamentoService(notaFiscalRepository, estoqueApiClient);
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarNotaFiscal([FromBody] CadastrarNotaFiscalRequest request)
    {
        try
        {
            var nota = await _faturamentoService.CadastrarNotaFiscalAsync(request.Numero);

            return CreatedAtAction(nameof(ObterNotaFiscal), new { id = nota.Id }, new
            {
                id = nota.Id,
                numero = nota.Numero,
                status = nota.Status,
                dataEmissao = nota.DataEmissao,
                valorTotal = nota.ValorTotal
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao cadastrar nota fiscal: {ex.Message}" });
        }
    }

    [HttpPost("{id}/itens")]
    public async Task<IActionResult> AdicionarProduto(Guid id, [FromBody] AdicionarProdutoRequest request)
    {
        try
        {
            var resultado = await _faturamentoService.AdicionarProdutoAsync(id, request.ProdutoId, request.Quantidade);

            if (!resultado.Sucesso)
                return BadRequest(new { mensagem = resultado.Mensagem });

            return Ok(new
            {
                mensagem = resultado.Mensagem,
                item = new
                {
                    id = resultado.Item?.Id,
                    produtoId = resultado.Item?.ProdutoId,
                    nomeProduto = resultado.Item?.NomeProduto,
                    quantidade = resultado.Item?.Quantidade,
                    precoUnitario = resultado.Item?.PrecoUnitario,
                    valorTotal = resultado.Item?.ValorTotal
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao adicionar produto: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarNotasFiscais()
    {
        try
        {
            var notas = await _notaFiscalRepository.ObterTodosAsync();

            var resultado = notas.Select(n => new
            {
                id = n.Id,
                numero = n.Numero,
                status = n.Status,
                dataEmissao = n.DataEmissao,
                dataFechamento = n.DataFechamento,
                valorTotal = n.ValorTotal,
                quantidadeItens = n.Itens.Count
            });

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao listar notas fiscais: {ex.Message}" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterNotaFiscal(Guid id)
    {
        try
        {
            var nota = await _notaFiscalRepository.ObterPorIdAsync(id);

            if (nota == null)
                return NotFound(new { mensagem = "Nota fiscal não encontrada" });

            return Ok(new
            {
                id = nota.Id,
                numero = nota.Numero,
                status = nota.Status,
                dataEmissao = nota.DataEmissao,
                dataFechamento = nota.DataFechamento,
                valorTotal = nota.ValorTotal,
                itens = nota.Itens.Select(i => new
                {
                    id = i.Id,
                    produtoId = i.ProdutoId,
                    nomeProduto = i.NomeProduto,
                    quantidade = i.Quantidade,
                    precoUnitario = i.PrecoUnitario,
                    valorTotal = i.ValorTotal
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao buscar nota fiscal: {ex.Message}" });
        }
    }

    [HttpPost("{id}/imprimir")]
    public async Task<IActionResult> ImprimirNotaFiscal(Guid id)
    {
        try
        {
            var resultado = await _faturamentoService.ImprimirNotaFiscalAsync(id);

            if (!resultado.Sucesso)
                return BadRequest(new { mensagem = resultado.Mensagem });

            return Ok(new
            {
                mensagem = resultado.Mensagem,
                notaFiscal = new
                {
                    id = resultado.NotaFiscal?.Id,
                    numero = resultado.NotaFiscal?.Numero,
                    status = resultado.NotaFiscal?.Status,
                    dataEmissao = resultado.NotaFiscal?.DataEmissao,
                    dataFechamento = resultado.NotaFiscal?.DataFechamento,
                    valorTotal = resultado.NotaFiscal?.ValorTotal,
                    itens = resultado.NotaFiscal?.Itens.Select(i => new
                    {
                        nomeProduto = i.NomeProduto,
                        quantidade = i.Quantidade,
                        precoUnitario = i.PrecoUnitario,
                        valorTotal = i.ValorTotal
                    })
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao imprimir nota fiscal: {ex.Message}" });
        }
    }
}

public class CadastrarNotaFiscalRequest
{
    public string Numero { get; set; } = string.Empty;
}

public class AdicionarProdutoRequest
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
}