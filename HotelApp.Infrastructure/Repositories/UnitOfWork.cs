using HotelApp.Domain.Contracts;
using HotelApp.Infrastructure.Identity.Data;
using HotelManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Repositories
{
    public class UnitOfWork(HotelDbContext dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> repositories = [];

        IGenericRepository<TEntity, TKey> IUnitOfWork.GetRepository<TEntity, TKey>()
        {
            var typeName = typeof(TEntity).Name;
            if (repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<TEntity, TKey>)value;
            else
            {
                var repo = new GenericRepository<TEntity, TKey>(dbContext);
                repositories[typeName] = repo;
                return repo;

            }
        }

        async Task<int> IUnitOfWork.SaveChanges(CancellationToken c) => await dbContext.SaveChangesAsync(c);
    }
}

