using HotelApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChanges(CancellationToken c = default);

        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
    }
}
