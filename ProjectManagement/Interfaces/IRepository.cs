using ProjectManagement.Domain.Models;
using System.Linq.Expressions;

namespace ProjectManagement.Api.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        void Insert(T entity);
        void InsertRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        IQueryable<T> Table { get; }
    }
}
