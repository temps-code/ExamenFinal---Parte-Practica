using System;
using System.Collections.Generic;

namespace Application.DTOs.Pedidos
{
    public class ProductoPedidoDto
    {
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class CreatePedidoDto
    {
        public List<ProductoPedidoDto> Productos { get; set; } = new();
    }

    public class PedidoDto
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public List<Guid> ProductosIds { get; set; } = new();
        public List<int> Cantidades { get; set; } = new();
        public decimal Total { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
