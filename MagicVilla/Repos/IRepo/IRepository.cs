using MagicVilla.Models;
using System.Linq.Expressions;

namespace MagicVilla.Repos.IRepo
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true);
        Task CreateAsync(T villa);
        Task RemoveAsync(T villa);
        Task SaveAsync();
    }
}
