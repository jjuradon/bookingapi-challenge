using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Booking.Core.Exceptions;
using Booking.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Data
{
    /// <summary>
    /// Repository pattern implementation for Sql Database.
    /// </summary>
    /// <typeparam name="TEntity">Entities representative type.</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly BookingContext _context;
        protected readonly DbSet<TEntity> _query;

        public Repository(BookingContext context)
        {
            _context = context;
            _query = _context.Set<TEntity>();
        }

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) => _query.AsAsyncEnumerable<TEntity>().GetAsyncEnumerator(cancellationToken);
        public Type ElementType => _query.AsQueryable().ElementType;
        public Expression Expression => _query.AsQueryable().Expression;
        public IQueryProvider Provider => _query.AsQueryable().Provider;
        public IEnumerator<TEntity> GetEnumerator() => _query.AsQueryable<TEntity>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _query.AsEnumerable().GetEnumerator();

        public virtual async Task Put(IEnumerable<TEntity> entities)
        {
            await Execute(async () => {
                _context.AddRange(entities);
                await _context.SaveChangesAsync(true);
            });
        }

        public virtual async Task Put(TEntity entity)
        {
            await Execute(async () => {
                _context.Add(entity);
                await _context.SaveChangesAsync();
            });
            
        }

        public virtual async Task Update(TEntity entity, Func<TEntity, TEntity, bool> mergeNewEntity = null)
        {
            
            await Execute(async () => {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            });
        }

        public virtual async Task Update(IEnumerable<TEntity> entities, Func<TEntity, TEntity, bool> mergeNewEntity = null)
        {
            await Execute(async () => {
                _context.UpdateRange(entities);
                await _context.SaveChangesAsync();
            });
        }

        public virtual async Task Delete(TEntity entity)
        {
            await Execute(async () => {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            });
        }

        public virtual async Task Delete(IEnumerable<TEntity> entities)
        {
            await Execute(async () => {
                _context.RemoveRange(entities);
                await _context.SaveChangesAsync();
            });
        }

        private async Task Execute(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (DbUpdateException ex)
            {
                throw new PersistenceException(ex.Message, ex);
            }
        }
    }
}