namespace ServiceEstoque.Domain.Entities;

public class ReservaProduto
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public Guid NotaFiscalId { get; set; }
    public int Quantidade { get; set; }
    public string Status { get; set; } = "Pendente";
    public DateTime DataReserva { get; set; }
    public DateTime? DataConfirmacao { get; set; }

    public Produto? Produto { get; set; }
}