using ProjectManagement.Domain.Entities;
using System.Linq.Expressions;
using Tasks = System.Threading.Tasks;

namespace ProjectManagement.Api.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAll();
        Tasks.Task Insert(T entity);
        Tasks.Task InsertRange(IEnumerable<T> entities);
        Tasks.Task Update(T entity);
        Tasks.Task Delete(T entity);
        Tasks.Task DeleteRange(IEnumerable<T> entities);
        IQueryable<T> Table { get; }
    }
}
