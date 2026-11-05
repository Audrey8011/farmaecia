using FarmaECia.Models;
using Microsoft.EntityFrameworkCore;

namespace FarmaECia.Data;

public class FarmaDbContext : DbContext
{
    public FarmaDbContext(DbContextOptions<FarmaDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Usuario>().Property(u => u.Email).HasMaxLength(180);
        modelBuilder.Entity<Produto>().Property(p => p.Preco).HasPrecision(12, 2);
        modelBuilder.Entity<Produto>().Property(p => p.PrecoAnterior).HasPrecision(12, 2);
        modelBuilder.Entity<Pedido>().Property(p => p.Total).HasPrecision(12, 2);
        modelBuilder.Entity<ItemPedido>().Property(i => i.PrecoUnitario).HasPrecision(12, 2);
        modelBuilder.Entity<Venda>().Property(v => v.Total).HasPrecision(12, 2);
        modelBuilder.Entity<ItemVenda>().Property(i => i.PrecoUnitario).HasPrecision(12, 2);
        modelBuilder.Entity<Pedido>().HasOne(p => p.Usuario).WithMany(u => u.Pedidos).HasForeignKey(p => p.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ItemPedido>().HasOne(i => i.Produto).WithMany(p => p.ItensPedido).HasForeignKey(i => i.ProdutoId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Venda>().HasIndex(v => v.PedidoId).IsUnique();
        modelBuilder.Entity<Venda>().HasOne(v => v.Pedido).WithOne(p => p.Venda).HasForeignKey<Venda>(v => v.PedidoId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Venda>().HasOne(v => v.Usuario).WithMany(u => u.Vendas).HasForeignKey(v => v.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ItemVenda>().HasOne(i => i.Produto).WithMany(p => p.ItensVenda).HasForeignKey(i => i.ProdutoId).OnDelete(DeleteBehavior.Restrict);
    }
}
