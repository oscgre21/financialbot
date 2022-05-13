using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinancialBot.Domain.Repositories
{
    public interface IRepository<T>  where T : class
    {
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate = null,
                int? page = null,
                int? pageSize = null,
                SortExpression<T> sortExpressions = null,
                bool trackEntities = true,
                params Expression<Func<T, object>>[] includeProperties);

        Task<T> First(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);
 
        void Add(T entity);
        void Add(params T[] entities);
        void Add(IEnumerable<T> entities);
         
        Task Delete(T entity);
        void Delete(params T[] entities);
        void Delete(IEnumerable<T> entities);
        void Update(T entity);
        void Update(params T[] entities);
        void Update(IEnumerable<T> entities);
        void Detached(T entity);
    }
}
