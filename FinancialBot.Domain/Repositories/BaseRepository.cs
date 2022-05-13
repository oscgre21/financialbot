using FinancialBot.Domain.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinancialBot.Domain.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    { 
        public readonly BaseDBContext _context;
        public readonly DbSet<T> _dbSet;
        public BaseRepository(BaseDBContext context)
        { 
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public Task<T> First(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> list = _dbSet.AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                list = list.Include(includeProperty);
            }
            return list.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate = null,
                int? page = null,
                int? pageSize = null,
                SortExpression<T> sortExpressions = null,
                bool trackEntities = true,
                params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (!trackEntities)
                query = query.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (sortExpressions != null)
            {
                IOrderedQueryable<T> orderedQuery = null;

                if (sortExpressions.SortDirection == ListSortDirection.Ascending)
                {
                    orderedQuery = query.OrderBy(sortExpressions.SortBy);
                }
                else
                {
                    orderedQuery = query.OrderByDescending(sortExpressions.SortBy);
                }


                if (page != null)
                {
                    query = orderedQuery.Skip(((int)page - 1) * (int)pageSize);
                }
            }

            if (pageSize != null)
            {
                query = query.Take((int)pageSize);
            }


            return await query.ToListAsync();
        } 
        public void Add(T entity)
        {
            _dbSet.AddAsync(entity);
        }
        public void Add(params T[] entities)
        {
            _dbSet.AddRange(entities);
        }
        public void Add(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

 
        public async Task Delete(T entity)
        {
            if (entity is null) throw new ArgumentNullException("Empty entity");
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public void Delete(params T[] entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public void Delete(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void Update(params T[] entities)
        {
            foreach (T entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        public virtual void Detached(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
         
         
    }
}
