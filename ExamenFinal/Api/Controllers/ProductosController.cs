using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Productos;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ProductosController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // GET api/productos
        [HttpGet]
        public async Task<IActionResult> GetActivos(CancellationToken ct)
        {
            var list = await _ctx.Productos.Where(p => p.IsActive).OrderBy(p => p.Nombre).ToListAsync(ct);
            var dtos = list.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
            return Ok(dtos);
        }

        // GET api/productos/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var list = await _ctx.Productos.OrderBy(p => p.Nombre).ToListAsync(ct);
            var dtos = list.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
            return Ok(dtos);
        }

        // GET api/productos/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var p = await _ctx.Productos.FindAsync(new object[] { id }, ct);
            if (p == null) return NotFound();
            var dto = new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            };
            return Ok(dto);
        }

        // POST api/productos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductoDto dto, CancellationToken ct)
        {
            if (dto == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(dto.Nombre)) return BadRequest(new { message = "Nombre requerido" });
            if (dto.Precio < 0) return BadRequest(new { message = "Precio inválido" });

            var p = new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _ctx.Productos.AddAsync(p, ct);
            await _ctx.SaveChangesAsync(ct);

            var result = new ProductoDto { Id = p.Id, Nombre = p.Nombre, Precio = p.Precio, IsActive = p.IsActive, CreatedAt = p.CreatedAt };
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, result);
        }

        // PUT api/productos/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductoDto dto, CancellationToken ct)
        {
            var p = await _ctx.Productos.FindAsync(new object[] { id }, ct);
            if (p == null) return NotFound();

            if (dto.Nombre != null) p.Nombre = dto.Nombre;
            if (dto.Precio.HasValue) p.Precio = dto.Precio.Value;
            if (dto.IsActive.HasValue) p.IsActive = dto.IsActive.Value;
            p.UpdatedAt = DateTime.UtcNow;

            _ctx.Productos.Update(p);
            await _ctx.SaveChangesAsync(ct);
            return NoContent();
        }

        // DELETE api/productos/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var p = await _ctx.Productos.FindAsync(new object[] { id }, ct);
            if (p == null) return NotFound();
            if (!p.IsActive) return BadRequest(new { message = "Ya está inactivo" });

            p.IsActive = false;
            p.UpdatedAt = DateTime.UtcNow;
            _ctx.Productos.Update(p);
            await _ctx.SaveChangesAsync(ct);

            return NoContent();
        }

        // POST api/productos/{id}/reactivar
        [HttpPost("{id:guid}/reactivar")]
        public async Task<IActionResult> Reactivar(Guid id, CancellationToken ct)
        {
            var p = await _ctx.Productos.FindAsync(new object[] { id }, ct);
            if (p == null) return NotFound();
            if (p.IsActive) return BadRequest(new { message = "Ya está activo" });

            p.IsActive = true;
            p.UpdatedAt = DateTime.UtcNow;
            _ctx.Productos.Update(p);
            await _ctx.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
