using MagicVilla.Models;
using System.Linq.Expressions;

namespace MagicVilla.Repos.IRepo
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(VillaNumber villa);
    }
}
