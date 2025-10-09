namespace ServiceFaturamento.Domain.Entities;

public class NotaFiscal
{
    public Guid Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string Status { get; set; } = "Aberta";
    public DateTime DataEmissao { get; set; }
    public DateTime? DataFechamento { get; set; }
    public int Version { get; set; }
    public decimal ValorTotal { get; set; }

    public List<ItemNotaFiscal> Itens { get; set; } = new();
}