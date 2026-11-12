
using Inventario.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Data
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(
            DbContextOptions<InventarioDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bodega> Bodegas { get; set; } = null!;

        public DbSet<Existencia> Existencias { get; set; } = null!;

        public DbSet<MovimientoInventario> Movimientos { get; set; } = null!;

        public DbSet<ReservaStock> Reservas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bodega>(entity =>
            {
                entity.ToTable("Bodega");
                entity.HasKey(e => e.IdBodega);
                entity.Property(e => e.IdBodega)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Existencia>(entity =>
            {
                entity.ToTable("Existencia");
                entity.HasKey(e => e.IdExistencia);

                entity.Property(e => e.IdExistencia)
                    .ValueGeneratedOnAdd();

                entity.HasOne<Bodega>()
                    .WithMany()
                    .HasForeignKey(e => e.IdBodega)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new
                {
                    e.IdProducto,
                    e.IdBodega
                }).IsUnique();
            });

            modelBuilder.Entity<MovimientoInventario>(entity =>
            {
                entity.ToTable("MovimientoInventario");
                entity.HasKey(e => e.IdMovimiento);

                entity.Property(e => e.IdMovimiento)
                    .ValueGeneratedOnAdd();

                entity.HasOne<Bodega>()
                    .WithMany()
                    .HasForeignKey(e => e.IdBodega)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReservaStock>(entity =>
            {
                entity.ToTable("ReservaStock");
                entity.HasKey(e => e.IdReserva);

                entity.Property(e => e.IdReserva)
                    .ValueGeneratedOnAdd();

                entity.HasOne<Bodega>()
                    .WithMany()
                    .HasForeignKey(e => e.IdBodega)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
