using MagicVilla.Models;
using System.Linq.Expressions;

namespace MagicVilla.Repos.IRepo
{
    public interface IVillaRepository : IRepository<Villa>
    {
        Task<Villa> UpdateAsync(Villa villa);
    }
}
