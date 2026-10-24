using HotelApp.Domain.Common;
using HotelApp.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TEntity, TKey>(
            IQueryable<TEntity> inputQuery,
            ISpecification<TEntity, TKey> spec) where TEntity : BaseEntity<TKey>
        {
            var query = inputQuery;

            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending is not null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsSplitQuery)
                query = query.AsSplitQuery();

            if (spec.AsNoTracking)
                query = query.AsNoTracking();

            return query;
        }
    }
}
