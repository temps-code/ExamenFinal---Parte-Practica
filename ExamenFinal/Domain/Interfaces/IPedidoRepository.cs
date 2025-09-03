using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPedidoRepository
    {
        Task AddAsync(Pedido pedido, CancellationToken ct = default);
        Task<Pedido?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Pedido>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Pedido>> GetAllActiveAsync(CancellationToken ct = default);
        Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default);
        Task<bool> ReactivateAsync(Guid id, CancellationToken ct = default);
    }
}
