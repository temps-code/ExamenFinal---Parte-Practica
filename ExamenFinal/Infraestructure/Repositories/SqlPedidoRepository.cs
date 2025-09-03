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
    public class SqlPedidoRepository : IPedidoRepository
    {
        private readonly AppDbContext _ctx;
        public SqlPedidoRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(Pedido pedido, CancellationToken ct = default)
        {
            if (pedido == null) throw new ArgumentNullException(nameof(pedido));
            if (pedido.Id == Guid.Empty) pedido.Id = Guid.NewGuid();
            pedido.CreatedAt = DateTime.UtcNow;
            await _ctx.Pedidos.AddAsync(pedido, ct);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync(CancellationToken ct = default)
            => await _ctx.Pedidos.OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

        public async Task<IEnumerable<Pedido>> GetAllActiveAsync(CancellationToken ct = default)
            => await _ctx.Pedidos.Where(p => p.IsActive).OrderByDescending(p => p.CreatedAt).ToListAsync(ct);

        public async Task<Pedido?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _ctx.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _ctx.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (existing == null || !existing.IsActive) return false;
            existing.IsActive = false;
            existing.UpdatedAt = DateTime.UtcNow;
            _ctx.Pedidos.Update(existing);
            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> ReactivateAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _ctx.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (existing == null || existing.IsActive) return false;
            existing.IsActive = true;
            existing.UpdatedAt = DateTime.UtcNow;
            _ctx.Pedidos.Update(existing);
            await _ctx.SaveChangesAsync(ct);
            return true;
        }
    }
}
