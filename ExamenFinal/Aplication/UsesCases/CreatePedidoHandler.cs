using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Pedidos;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases.Pedidos
{
    public class CreatePedidoHandler
    {
        private readonly IPedidoRepository _pedidoRepo;
        private readonly IProductoRepository _productoRepo;

        public CreatePedidoHandler(IPedidoRepository pedidoRepo, IProductoRepository productoRepo)
        {
            _pedidoRepo = pedidoRepo ?? throw new ArgumentNullException(nameof(pedidoRepo));
            _productoRepo = productoRepo ?? throw new ArgumentNullException(nameof(productoRepo));
        }

        public async Task<Pedido> HandleAsync(CreatePedidoDto dto, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Productos == null || !dto.Productos.Any())
                throw new ArgumentException("Debe enviar al menos un producto (productoId + cantidad).");

            var seen = new HashSet<Guid>();
            foreach (var p in dto.Productos)
            {
                if (p.Cantidad <= 0)
                    throw new ArgumentException($"Cantidad inválida para el producto {p.ProductoId}. Debe ser mayor que 0.");

                if (!seen.Add(p.ProductoId))
                    throw new ArgumentException($"Producto {p.ProductoId} aparece repetido. No se permiten duplicados.");
            }

            var productosIds = new List<Guid>();
            var cantidades = new List<int>();
            decimal total = 0m;

            foreach (var item in dto.Productos)
            {
                var producto = await _productoRepo.GetByIdAsync(item.ProductoId, ct);
                if (producto == null || !producto.IsActive)
                    throw new KeyNotFoundException($"Producto {item.ProductoId} no encontrado o inactivo.");

                productosIds.Add(producto.Id);
                cantidades.Add(item.Cantidad);

                var subtotal = Math.Round(producto.Precio * item.Cantidad, 2);
                total += subtotal;
            }

            total = Math.Round(total, 2);

            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ProductosIds = productosIds,
                Cantidades = cantidades,
                Total = total,
                Fecha = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _pedidoRepo.AddAsync(pedido, ct);

            return pedido;
        }
    }
}
