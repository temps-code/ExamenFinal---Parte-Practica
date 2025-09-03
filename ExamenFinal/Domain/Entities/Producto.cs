using System;

namespace Domain.Entities
{
    public class Producto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
