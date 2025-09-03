using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SqlProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _ctx;
        public SqlProductoRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(Producto producto, CancellationToken ct = default)
        {
            if (producto == null) throw new ArgumentNullException(nameof(producto));
            if (producto.Id == Guid.Empty) producto.Id = Guid.NewGuid();
            producto.CreatedAt = DateTime.UtcNow;
            await _ctx.Productos.AddAsync(producto, ct);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Producto>> GetAllAsync(CancellationToken ct = default)
            => await _ctx.Productos.OrderBy(p => p.Nombre).ToListAsync(ct);

        public async Task<IEnumerable<Producto>> GetAllActiveAsync(CancellationToken ct = default)
            => await _ctx.Productos.Where(p => p.IsActive).OrderBy(p => p.Nombre).ToListAsync(ct);

        public async Task<Producto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _ctx.Productos.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<bool> UpdateAsync(Producto producto, CancellationToken ct = default)
        {
            if (producto == null) throw new ArgumentNullException(nameof(producto));
            var existing = await _ctx.Productos.FirstOrDefaultAsync(p => p.Id == producto.Id, ct);
            if (existing == null) return false;

            existing.Nombre = producto.Nombre;
            existing.Precio = producto.Precio;
            existing.UpdatedAt = DateTime.UtcNow;

            _ctx.Productos.Update(existing);
            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _ctx.Productos.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (existing == null || !existing.IsActive) return false;
            existing.IsActive = false;
            existing.UpdatedAt = DateTime.UtcNow;
            _ctx.Productos.Update(existing);
            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ReactivateAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _ctx.Productos.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (existing == null || existing.IsActive) return false;
            existing.IsActive = true;
            existing.UpdatedAt = DateTime.UtcNow;
            _ctx.Productos.Update(existing);
            await _ctx.SaveChangesAsync(ct);
            return true;
        }
    }
}
