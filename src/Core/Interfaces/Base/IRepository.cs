using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Core.Interfaces
{
    public interface IRepository<T> : IOrderedQueryable<T>, IAsyncEnumerable<T>
    {
        Task Delete(T entity);
        Task Delete(IEnumerable<T> entities);
        Task Put(IEnumerable<T> entities);
        Task Put(T entity);
        Task Update(T entity, Func<T, T, bool> mergeNewEntity = null);
        Task Update(IEnumerable<T> entities, Func<T, T, bool> mergeNewEntity = null);
    }
}