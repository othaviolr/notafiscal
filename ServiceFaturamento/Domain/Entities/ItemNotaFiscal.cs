namespace ServiceFaturamento.Domain.Entities;

public class ItemNotaFiscal
{
    public Guid Id { get; set; }
    public Guid NotaFiscalId { get; set; }
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal => Quantidade * PrecoUnitario;

    public NotaFiscal? NotaFiscal { get; set; }
}