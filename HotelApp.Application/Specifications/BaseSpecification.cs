using HotelApp.Domain.Common;
using HotelApp.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HotelApp.Application.Specifications
{
    public abstract class BaseSpecification<TEntity, TKey> : ISpecification<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected BaseSpecification() { }

        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
        public List<Expression<Func<TEntity, object>>> Includes { get; } = [];
        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }
        public bool IsSplitQuery { get; private set; }
        public bool AsNoTracking { get; private set; }

        protected void AddInclude(Expression<Func<TEntity, object>> include) => Includes.Add(include);
        protected void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy) => OrderBy = orderBy;
        protected void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderBy) => OrderByDescending = orderBy;
        protected void EnableSplitQuery() => IsSplitQuery = true;
        protected void EnableNoTracking() => AsNoTracking = true;
    }
}
