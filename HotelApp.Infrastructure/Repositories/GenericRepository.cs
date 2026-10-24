using HotelApp.Domain.Common;
using HotelApp.Domain.Contracts;
using HotelApp.Infrastructure.Identity.Data;
using HotelApp.Infrastructure.Specifications;
using HotelManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Repositories
{


    public class GenericRepository<TEntity, TKey>(HotelDbContext dbContext) : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public void Add(TEntity entity, CancellationToken c = default) 
            => dbContext.Set<TEntity>().Add(entity);

        public void Delete(TEntity entity, CancellationToken c = default) 
            => dbContext.Set<TEntity>().Remove(entity);

        public async Task<TEntity?> GetByIdAsync(TKey Id, CancellationToken c = default)
            => await dbContext.Set<TEntity>().FindAsync([Id], c);

        public void Update(TEntity entity, CancellationToken c = default) 
            => dbContext.Set<TEntity>().Update(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity, TKey> spec, CancellationToken c = default)
            => await ApplySpec(spec).ToListAsync(c);

        public async Task<TEntity?> GetAsync(ISpecification<TEntity, TKey> spec, CancellationToken c = default)
            => await ApplySpec(spec).FirstOrDefaultAsync(c);

        public async Task<bool> AnyAsync(ISpecification<TEntity, TKey> spec, CancellationToken c = default)
            => await ApplySpec(spec).AnyAsync(c);

        private IQueryable<TEntity> ApplySpec(ISpecification<TEntity, TKey> spec)
            => SpecificationEvaluator.GetQuery(dbContext.Set<TEntity>().AsQueryable(), spec);
    }
}
