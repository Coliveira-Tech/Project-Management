using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Interfaces;
using ProjectManagement.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Tasks = System.Threading.Tasks;

namespace ProjectManagement.Api.Infra.Data
{
    [ExcludeFromCodeCoverage]
    public class Repository<T>(Context context) : IRepository<T> where T : BaseEntity
    {
        private readonly Context context = context;
        private DbSet<T> entity = context.Set<T>();

        public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate)
        {
            return await entity.Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await entity.ToListAsync();
        }
        
        public async Tasks.Task Insert(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.entity.Add(entity);
            await context.SaveChangesAsync();
        }
        public async Tasks.Task InsertRange(IEnumerable<T> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            this.entity.AddRange(entities);
            await context.SaveChangesAsync();
        }
        public async Tasks.Task Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await context.SaveChangesAsync();
        }
        public async Tasks.Task Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.entity.Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Tasks.Task DeleteRange(IEnumerable<T> entities)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.entity.RemoveRange(entities);
            await context.SaveChangesAsync();
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        protected virtual DbSet<T> Entities
        {
            get
            {
                entity ??= context.Set<T>();
                return entity;
            }
        }
    }
}
