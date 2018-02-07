using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Urf.Core.Abstractions;

namespace URF.Core.EF
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext Context { get; }
        protected DbSet<TEntity> Set { get; }

        public Repository(DbContext context)
        {
            Context = context;
            Set = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> SelectAsync(CancellationToken cancellationToken = default)
            => await Set.ToListAsync(cancellationToken);

        public virtual async Task<IEnumerable<TEntity>> SelectSqlAsync(string sql, object[] parameters, CancellationToken cancellationToken = default)
            => await Set.FromSql(sql, (object[])parameters).ToListAsync(cancellationToken);

        public virtual async Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
            => await Set.FindAsync((object[])keyValues, cancellationToken);

        public virtual async Task<TEntity> FindAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
            => await FindAsync(new object[] { keyValue }, cancellationToken);

        public virtual async Task<bool> ExistsAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var item = await FindAsync(keyValues, cancellationToken);
            return item != null;
        }

        public virtual async Task<bool> ExistsAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
            => await ExistsAsync(new object[] {keyValue}, cancellationToken);

        public virtual async Task LoadPropertyAsync(TEntity item, Expression<Func<TEntity, object>> property, CancellationToken cancellationToken = default)
            => await Context.Entry(item).Reference(property).LoadAsync(cancellationToken);

        public virtual void Attach(TEntity item)
            => Set.Attach(item);

        public virtual void Detach(TEntity item)
            => Context.Entry(item).State = EntityState.Detached;

        public virtual void Insert(TEntity item)
            => Context.Entry(item).State = EntityState.Added;

        public virtual void Update(TEntity item)
            => Context.Entry(item).State = EntityState.Modified;

        public virtual void Delete(TEntity item)
            => Context.Entry(item).State = EntityState.Deleted;

        public virtual async Task<bool> DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var item = await FindAsync(keyValues, cancellationToken);
            if (item == null) return false;
            Context.Entry(item).State = EntityState.Deleted;
            return true;
        }

        public virtual async Task<bool> DeleteAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
            => await DeleteAsync(new object[] { keyValue }, cancellationToken);

        public virtual IQueryable<TEntity> Queryable() => Set;

        public virtual IQueryable<TEntity> QueryableSql(string sql, params object[] parameters)
            => Set.FromSql(sql, parameters);

        public virtual async Task<IEnumerable<TEntity>> SelectAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, object>>[] includes = null,
            SortExpression<TEntity>[] sortExpressions = null,
            int? page = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = Set;

            if (filter != null)
                query = query.Where(filter);

            if (includes != null)
                foreach (var include in includes)
                    query.Include(include);

            if (sortExpressions != null)
            {
                IOrderedQueryable<TEntity> orderedQuery = null;

                for (var i = 0; i < sortExpressions.Count(); i++)
                    orderedQuery = sortExpressions[i].SortDirection == ListSortDirection.Ascending 
                        ? (i == 0 ? query.OrderBy(sortExpressions[i].SortBy) 
                            : orderedQuery.ThenBy(sortExpressions[i].SortBy)) 
                        : (i == 0 ? query.OrderByDescending(sortExpressions[i].SortBy) 
                            : orderedQuery.ThenByDescending(sortExpressions[i].SortBy));

                if (pageSize.HasValue && page.HasValue)
                    query = orderedQuery.Skip((page.Value - 1) * pageSize.Value);
            }

            if (pageSize.HasValue)
                query = query.Take(pageSize.Value);

            return await query.ToListAsync(cancellationToken);
        }
    }

    public class SortExpression<TEntity> where TEntity : class
    {
        public SortExpression(Expression<Func<TEntity, object>> sortBy, ListSortDirection sortDirection)
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
        }

        public Expression<Func<TEntity, object>> SortBy { get; set; }
        public ListSortDirection SortDirection { get; set; }
    }
}
