namespace ServiceEstoque.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int SaldoDisponivel { get; set; }
    public int SaldoReservado { get; set; }
    public int Version { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }

    public int SaldoTotal => SaldoDisponivel + SaldoReservado;
}