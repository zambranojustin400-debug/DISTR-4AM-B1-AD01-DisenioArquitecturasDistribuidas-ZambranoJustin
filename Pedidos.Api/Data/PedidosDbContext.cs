
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Models;

namespace Pedidos.Api.Data
{
    public class PedidosDbContext : DbContext
    {
        public PedidosDbContext(
            DbContextOptions<PedidosDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;

        public DbSet<Pedido> Pedidos { get; set; } = null!;

        public DbSet<DetallePedido> DetallesPedido { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TABLA CLIENTE
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");

                entity.HasKey(e => e.IdCliente);

                entity.Property(e => e.IdCliente)
                    .ValueGeneratedOnAdd();

                entity.HasIndex(e => e.Cedula)
                    .IsUnique();
            });

            // TABLA PEDIDO
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("Pedido");

                entity.HasKey(e => e.IdPedido);

                entity.Property(e => e.IdPedido)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Total)
                    .HasPrecision(18, 2);

                entity.HasOne<Cliente>()
                    .WithMany()
                    .HasForeignKey(e => e.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TABLA DETALLE PEDIDO
            modelBuilder.Entity<DetallePedido>(entity =>
            {
                entity.ToTable("DetallePedido");

                entity.HasKey(e => e.IdDetalle);

                entity.Property(e => e.IdDetalle)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.PrecioUnitario)
                    .HasPrecision(18, 2);

                entity.HasOne<Pedido>()
                    .WithMany()
                    .HasForeignKey(e => e.IdPedido)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
