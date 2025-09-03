using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Pedido> Pedidos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            modelBuilder.Entity<Pedido>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.Total).HasPrecision(18, 2).IsRequired();
                b.Property(p => p.Fecha).IsRequired();

                b.Property(p => p.ProductosIds)
                 .HasConversion(
                     v => JsonSerializer.Serialize(v, jsonOptions),
                     v => JsonSerializer.Deserialize<List<Guid>>(v, jsonOptions) ?? new List<Guid>()
                 )
                 .HasColumnName("ProductosJson")
                 .HasColumnType("nvarchar(max)");

                b.Property(p => p.Cantidades)
                 .HasConversion(
                     v => JsonSerializer.Serialize(v, jsonOptions),
                     v => JsonSerializer.Deserialize<List<int>>(v, jsonOptions) ?? new List<int>()
                 )
                 .HasColumnName("CantidadesJson")
                 .HasColumnType("nvarchar(max)");

                b.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
                b.Property(p => p.CreatedAt).IsRequired();
                b.Property(p => p.UpdatedAt);

                b.HasIndex(p => p.Fecha);
            });

            modelBuilder.Entity<Producto>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Nombre).IsRequired().HasMaxLength(200);
                b.Property(x => x.Precio).HasPrecision(18, 2).IsRequired();
                b.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
            });
        }
    }
}
