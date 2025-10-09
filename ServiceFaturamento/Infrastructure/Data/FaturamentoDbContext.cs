using Microsoft.EntityFrameworkCore;
using ServiceFaturamento.Domain.Entities;

namespace ServiceFaturamento.Infrastructure.Data;

public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options)
    {
    }

    public DbSet<NotaFiscal> NotasFiscais { get; set; }
    public DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotaFiscal>(entity =>
        {
            entity.ToTable("notas_fiscais");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Numero)
                .HasColumnName("numero")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20);

            entity.Property(e => e.DataEmissao)
                .HasColumnName("data_emissao");

            entity.Property(e => e.DataFechamento)
                .HasColumnName("data_fechamento");

            entity.Property(e => e.Version)
                .HasColumnName("version")
                .IsConcurrencyToken();

            entity.Property(e => e.ValorTotal)
                .HasColumnName("valor_total")
                .HasColumnType("decimal(18,2)");

            entity.HasMany(e => e.Itens)
                .WithOne(i => i.NotaFiscal)
                .HasForeignKey(i => i.NotaFiscalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemNotaFiscal>(entity =>
        {
            entity.ToTable("itens_nota_fiscal");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.NotaFiscalId)
                .HasColumnName("nota_fiscal_id");

            entity.Property(e => e.ProdutoId)
                .HasColumnName("produto_id");

            entity.Property(e => e.NomeProduto)
                .HasColumnName("nome_produto")
                .HasMaxLength(200);

            entity.Property(e => e.Quantidade)
                .HasColumnName("quantidade");

            entity.Property(e => e.PrecoUnitario)
                .HasColumnName("preco_unitario")
                .HasColumnType("decimal(18,2)");
        });
    }
}