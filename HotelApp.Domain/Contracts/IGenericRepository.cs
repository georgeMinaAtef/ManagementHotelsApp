using HotelApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Contracts
{
    public interface IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        void Add(TEntity entity, CancellationToken c = default);

        void Update(TEntity entity, CancellationToken c = default);

        void Delete(TEntity entity, CancellationToken c = default);

        Task<TEntity?> GetByIdAsync(Tkey Id, CancellationToken c = default);

        // New: specification-based queries
        Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity, Tkey> spec, CancellationToken c = default);

        Task<TEntity?> GetAsync(ISpecification<TEntity, Tkey> spec, CancellationToken c = default);

        Task<bool> AnyAsync(ISpecification<TEntity, Tkey> spec, CancellationToken c = default);
    }
}

