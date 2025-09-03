using System;

namespace Application.DTOs.Productos
{
    public class ProductoDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProductoDto
    {
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
    }

    public class UpdateProductoDto
    {
        public string? Nombre { get; set; }
        public decimal? Precio { get; set; }
        public bool? IsActive { get; set; }
    }
}
