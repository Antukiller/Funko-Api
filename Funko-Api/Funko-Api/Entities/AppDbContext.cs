using System;
using Funko_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Funko_Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<Funko> Funkos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Funko>(e => {
            e.ToTable("funko");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
            e.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(20, 2);
            e.Property(x => x.Stock)
                .IsRequired();
            e.Property<long>("CategoriaId")
                .IsRequired();
            e.HasOne(x => x.Categoria)
                .WithMany(x => x.Funkos)
                .HasForeignKey("CategoriaId")
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.CreatedAt)
                .IsRequired();
            e.Property(x => x.UpdatedAt)
                .IsRequired();
            e.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            e.HasQueryFilter(x => !x.IsDeleted);

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            e.HasData(
                new { Id = 1L, Name = "Naruto Uzumaki", Price = 16.99m, Stock = 12, CategoriaId = 1L, CreatedAt = seedDate, UpdatedAt = seedDate, IsDeleted = false },
                new { Id = 2L, Name = "Sailor Moon", Price = 18.50m, Stock = 7, CategoriaId = 1L, CreatedAt = seedDate, UpdatedAt = seedDate, IsDeleted = false },
                new { Id = 3L, Name = "Lara Croft", Price = 20.99m, Stock = 6, CategoriaId = 2L, CreatedAt = seedDate, UpdatedAt = seedDate, IsDeleted = false },
                new { Id = 4L, Name = "Sonic", Price = 13.50m, Stock = 15, CategoriaId = 2L, CreatedAt = seedDate, UpdatedAt = seedDate, IsDeleted = false },
                new { Id = 5L, Name = "Wonder Woman", Price = 22.00m, Stock = 4, CategoriaId = 3L, CreatedAt = seedDate, UpdatedAt = seedDate, IsDeleted = false }
            );
        });

        modelBuilder.Entity<Categoria>(e => {
            e.ToTable("categoria");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            e.HasData(
                new Categoria { Id = 1, Name = "Anime" },
                new Categoria { Id = 2, Name = "Videojuegos" },
                new Categoria { Id = 3, Name = "Cómics" }
            );
        });
    }
}
