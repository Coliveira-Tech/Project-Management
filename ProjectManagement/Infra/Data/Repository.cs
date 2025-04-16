using ProjectManagement.Domain.Models;
using ProjectManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ProjectManagement.Api.Infra.Data
{
    [ExcludeFromCodeCoverage]
    public class Repository<T>(Context context) : IRepository<T> where T : BaseEntity
    {
        private readonly Context context = context;
        private DbSet<T> entity = context.Set<T>();

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return entity.Where(predicate).ToList();
        }
        public IEnumerable<T> GetAll()
        {
            return entity.ToList();
        }
        
        public void Insert(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.entity.Add(entity);
            context.SaveChanges();
        }
        public void InsertRange(IEnumerable<T> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);
            this.entity.AddRange(entities);
            context.SaveChanges();
        }
        public void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            context.SaveChanges();
        }
        public void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.entity.Remove(entity);
            context.SaveChanges();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.entity.RemoveRange(entities);
            context.SaveChanges();
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
