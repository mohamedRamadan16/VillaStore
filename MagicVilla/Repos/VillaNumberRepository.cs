using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Repos.IRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla.Repos
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<VillaNumber> UpdateAsync(VillaNumber villa)
        {
            villa.UpdatedDate = DateTime.Now;
            _db.VillasNumbers.Update(villa);
            await _db.SaveChangesAsync();
            return villa;
        }

    }
}
