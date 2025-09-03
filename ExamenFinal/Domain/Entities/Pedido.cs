using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<Guid> ProductosIds { get; set; } = new List<Guid>();
        public List<int> Cantidades { get; set; } = new List<int>();
        public decimal Total { get; set; } = 0m;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
