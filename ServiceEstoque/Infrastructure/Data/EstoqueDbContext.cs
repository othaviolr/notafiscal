using Microsoft.EntityFrameworkCore;
using ServiceEstoque.Domain.Entities;

namespace ServiceEstoque.Infrastructure.Data;

public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options)
    {
    }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ReservaProduto> ReservasProdutos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("produtos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Nome)
                .HasColumnName("nome")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(500);

            entity.Property(e => e.Preco)
                .HasColumnName("preco")
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.SaldoDisponivel)
                .HasColumnName("saldo_disponivel");

            entity.Property(e => e.SaldoReservado)
                .HasColumnName("saldo_reservado");

            entity.Property(e => e.Version)
                .HasColumnName("version")
                .IsConcurrencyToken();

            entity.Property(e => e.DataCriacao)
                .HasColumnName("data_criacao");

            entity.Property(e => e.DataAtualizacao)
                .HasColumnName("data_atualizacao");
        });

        modelBuilder.Entity<ReservaProduto>(entity =>
        {
            entity.ToTable("reservas_produtos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.ProdutoId)
                .HasColumnName("produto_id");

            entity.Property(e => e.NotaFiscalId)
                .HasColumnName("nota_fiscal_id");

            entity.Property(e => e.Quantidade)
                .HasColumnName("quantidade");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(50);

            entity.Property(e => e.DataReserva)
                .HasColumnName("data_reserva");

            entity.Property(e => e.DataConfirmacao)
                .HasColumnName("data_confirmacao");

            entity.HasOne(e => e.Produto)
                .WithMany()
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}