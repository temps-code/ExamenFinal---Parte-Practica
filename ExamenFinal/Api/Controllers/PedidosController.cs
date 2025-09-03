using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Pedidos;
using Application.UseCases.Pedidos;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly CreatePedidoHandler _createHandler;
        private readonly IPedidoRepository _pedidoRepo;

        public PedidosController(CreatePedidoHandler createHandler, IPedidoRepository pedidoRepo)
        {
            _createHandler = createHandler;
            _pedidoRepo = pedidoRepo;
        }

        // POST /api/pedidos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePedidoDto dto, CancellationToken ct)
        {
            try
            {
                var pedido = await _createHandler.HandleAsync(dto, ct);
                var result = new PedidoDto
                {
                    Id = pedido.Id,
                    Fecha = pedido.Fecha,
                    ProductosIds = pedido.ProductosIds,
                    Cantidades = pedido.Cantidades,
                    Total = pedido.Total,
                    IsActive = pedido.IsActive,
                    CreatedAt = pedido.CreatedAt
                };
                return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, result);
            }
            catch (ArgumentException aex) { return BadRequest(new { message = aex.Message }); }
            catch (KeyNotFoundException knf) { return NotFound(new { message = knf.Message }); }
        }

        // GET /api/pedidos/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var pedido = await _pedidoRepo.GetByIdAsync(id, ct);
            if (pedido == null) return NotFound();

            var dto = new PedidoDto
            {
                Id = pedido.Id,
                Fecha = pedido.Fecha,
                ProductosIds = pedido.ProductosIds,
                Cantidades = pedido.Cantidades,
                Total = pedido.Total,
                IsActive = pedido.IsActive,
                CreatedAt = pedido.CreatedAt
            };
            return Ok(dto);
        }

        // GET /api/pedidos
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _pedidoRepo.GetAllAsync(ct);
            var dtos = list.Select(p => new PedidoDto
            {
                Id = p.Id,
                Fecha = p.Fecha,
                ProductosIds = p.ProductosIds,
                Cantidades = p.Cantidades,
                Total = p.Total,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
            return Ok(dtos);
        }

        // GET /api/pedidos/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive(CancellationToken ct)
        {
            var list = await _pedidoRepo.GetAllActiveAsync(ct);
            var dtos = list.Select(p => new PedidoDto
            {
                Id = p.Id,
                Fecha = p.Fecha,
                ProductosIds = p.ProductosIds,
                Cantidades = p.Cantidades,
                Total = p.Total,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
            return Ok(dtos);
        }

        // POST /api/pedidos/{id}/deactivate
        [HttpPost("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var ok = await _pedidoRepo.DeactivateAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }

        // POST /api/pedidos/{id}/reactivate
        [HttpPost("{id:guid}/reactivate")]
        public async Task<IActionResult> Reactivate(Guid id, CancellationToken ct)
        {
            var ok = await _pedidoRepo.ReactivateAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
