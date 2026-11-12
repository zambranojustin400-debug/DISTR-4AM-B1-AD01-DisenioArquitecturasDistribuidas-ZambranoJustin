
using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Catalogo.Api.Data
{
    public class CatalogoDbContext : DbContext
    {
        public CatalogoDbContext(
            DbContextOptions<CatalogoDbContext> options)
            : base(options)
        {
        }

        // Tablas de la base de datos
        public DbSet<Categoria> Categorias { get; set; } = null!;

        public DbSet<Marca> Marcas { get; set; } = null!;

        public DbSet<Producto> Productos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tabla Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categoria");

                entity.HasKey(e => e.IdCategoria);

                entity.Property(e => e.IdCategoria)
                    .ValueGeneratedOnAdd();
            });

            // Tabla Marca
            modelBuilder.Entity<Marca>(entity =>
            {
                entity.ToTable("Marca");

                entity.HasKey(e => e.IdMarca);

                entity.Property(e => e.IdMarca)
                    .ValueGeneratedOnAdd();
            });

            // Tabla Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Producto");

                entity.HasKey(e => e.IdProducto);

                entity.Property(e => e.IdProducto)
                    .ValueGeneratedOnAdd();

                // SKU unico por producto
                entity.HasIndex(e => e.SKU)
                    .IsUnique();

                // Relacion Producto - Categoria
                entity.HasOne<Categoria>()
                    .WithMany()
                    .HasForeignKey(e => e.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relacion Producto - Marca
                entity.HasOne<Marca>()
                    .WithMany()
                    .HasForeignKey(e => e.IdMarca)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
