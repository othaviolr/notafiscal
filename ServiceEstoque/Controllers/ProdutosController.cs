using Microsoft.AspNetCore.Mvc;
using ServiceEstoque.Application.Services;
using ServiceEstoque.Domain.Repositories;

namespace ServiceEstoque.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly EstoqueService _estoqueService;

    public ProdutosController(
        IProdutoRepository produtoRepository,
        IReservaProdutoRepository reservaRepository)
    {
        _produtoRepository = produtoRepository;
        _estoqueService = new EstoqueService(produtoRepository, reservaRepository);
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarProduto([FromBody] CadastrarProdutoRequest request)
    {
        try
        {
            var produto = await _estoqueService.CadastrarProdutoAsync(
                request.Nome,
                request.Descricao,
                request.Preco,
                request.SaldoInicial
            );

            return CreatedAtAction(nameof(ObterProduto), new { id = produto.Id }, new
            {
                id = produto.Id,
                nome = produto.Nome,
                descricao = produto.Descricao,
                preco = produto.Preco,
                saldoDisponivel = produto.SaldoDisponivel,
                saldoReservado = produto.SaldoReservado,
                saldoTotal = produto.SaldoTotal
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao cadastrar produto: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarProdutos()
    {
        try
        {
            var produtos = await _produtoRepository.ObterTodosAsync();

            var resultado = produtos.Select(p => new
            {
                id = p.Id,
                nome = p.Nome,
                descricao = p.Descricao,
                preco = p.Preco,
                saldoDisponivel = p.SaldoDisponivel,
                saldoReservado = p.SaldoReservado,
                saldoTotal = p.SaldoTotal
            });

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao listar produtos: {ex.Message}" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterProduto(Guid id)
    {
        try
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound(new { mensagem = "Produto não encontrado" });

            return Ok(new
            {
                id = produto.Id,
                nome = produto.Nome,
                descricao = produto.Descricao,
                preco = produto.Preco,
                saldoDisponivel = produto.SaldoDisponivel,
                saldoReservado = produto.SaldoReservado,
                saldoTotal = produto.SaldoTotal
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensagem = $"Erro ao buscar produto: {ex.Message}" });
        }
    }
}

public class CadastrarProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int SaldoInicial { get; set; }
}