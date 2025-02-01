using MagicVilla.Data;
using MagicVilla.Loggings;
using MagicVilla.Models;
using MagicVilla.Models.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaAPIController:ControllerBase
    {
        private readonly ApplicationDbContext _db;

        //private readonly ILogger<VillaAPIController> _logger;
        /// For A Custom Logging
        private readonly ILogging _logger;

        public VillaAPIController(ILogging logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Villa>> GetAllVillas()
        {
            _logger.Log("Getting All Villas :D", "");
            return Ok(_db.Villas.AsNoTracking().ToList());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Villa> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.Log($"Error Getting Villa With ID : {id}", "Error");
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (villa == null)
                return NotFound();
            return Ok(villa); 
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult AddVilla(VillaDTO villa)
        {
            if(villa == null)
                return BadRequest();

            // custome validation
            if(_db.Villas.FirstOrDefault(v => v.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Name Must Be Unique!");
                return BadRequest(ModelState);
            }

            Villa model = new Villa()
            {
                Name = villa.Name,
                Details = villa.Details,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity,
            };
            _db.Villas.Add(model);
            _db.SaveChanges();
            return CreatedAtAction("GetVilla", new {id = villa.Id}, villa);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteVilla(int id)
        {
            if(id <= 0)
                return BadRequest();

            Villa villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if(villa == null) 
                return NotFound();

            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut]
        public IActionResult UpdateVilla([FromBody] VillaDTO villaFromRequest)
        {
            if(villaFromRequest == null || villaFromRequest.Id <= 0)
                return BadRequest();

            var model = _db.Villas.FirstOrDefault(v => v.Id == villaFromRequest.Id);
            if (model == null)
                return NotFound();

            model.Id = villaFromRequest.Id;
            model.Name = villaFromRequest.Name;
            model.Details = villaFromRequest.Details;
            model.Rate = villaFromRequest.Rate;
            model.Sqft = villaFromRequest.Sqft;
            model.Occupancy = villaFromRequest.Occupancy;
            model.ImageUrl = villaFromRequest.ImageUrl;
            model.Amenity = villaFromRequest.Amenity;
            model.UpdatedDate = DateTime.Now;
           
            _db.Villas.Update(model);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult UpdateCertainField(int id, JsonPatchDocument<VillaDTO> VillaFromRequest)
        {
            if(id <= 0 || VillaFromRequest == null)
                return BadRequest();

            Villa villaFromDb = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();

            VillaDTO villaDTO = new VillaDTO()
            {
                Name = villaFromDb.Name,
                Details = villaFromDb.Details,
                Rate = villaFromDb.Rate,
                Sqft = villaFromDb.Sqft,
                Occupancy = villaFromDb.Occupancy,
                ImageUrl = villaFromDb.ImageUrl,
                Amenity = villaFromDb.Amenity,
            };

            VillaFromRequest.ApplyTo(villaDTO, ModelState);
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            villaFromDb.Name = villaDTO.Name;
            villaFromDb.Details = villaDTO.Details;
            villaFromDb.Rate = villaDTO.Rate;
            villaFromDb.Sqft = villaDTO.Sqft;
            villaFromDb.Occupancy = villaDTO.Occupancy;
            villaFromDb.ImageUrl = villaDTO.ImageUrl;
            villaFromDb.Amenity = villaDTO.Amenity;
            villaFromDb.UpdatedDate = DateTime.Now;

            _db.Villas.Update(villaFromDb);
            _db.SaveChanges();

            return NoContent();
        }

    }
}
