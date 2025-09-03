using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductoRepository
    {
        Task<Producto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Producto>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<Producto>> GetAllActiveAsync(CancellationToken ct = default);

        Task AddAsync(Producto producto, CancellationToken ct = default);
        Task<bool> UpdateAsync(Producto producto, CancellationToken ct = default);

        Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default);
        Task<bool> ReactivateAsync(Guid id, CancellationToken ct = default);
    }
}
