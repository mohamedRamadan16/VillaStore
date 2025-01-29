using MagicVilla.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaAPIController:ControllerBase
    {
        [HttpGet]
        public IEnumerable<Villa> GetAllVillas()
        {
            return new List<Villa>()
            {
                new Villa() {Id = 1, Name = "RRVS19"},
                new Villa() {Id = 2, Name = "RRVS18"},
                new Villa() {Id = 3, Name = "RRVS17"}
            };
        }
    }
}
