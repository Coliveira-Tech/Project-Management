using ProjectManagement.Domain.Models;
using System.Linq.Expressions;

namespace ProjectManagement.Api.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAll();
        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Update(T entity);
        Task Delete(T entity);
        Task DeleteRange(IEnumerable<T> entities);
        IQueryable<T> Table { get; }
    }
}
