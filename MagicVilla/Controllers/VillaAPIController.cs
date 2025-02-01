using AutoMapper;
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
        private readonly IMapper _mapper;

        //private readonly ILogger<VillaAPIController> _logger;
        /// For A Custom Logging
        private readonly ILogging _logger;

        public VillaAPIController(ILogging logger, ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetAllVillas()
        {
            _logger.Log("Getting All Villas :D", "");
            IEnumerable <Villa> villaList = await _db.Villas.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.Log($"Error Getting Villa With ID : {id}", "Error");
                return BadRequest();
            }
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
                return NotFound();
            return Ok(_mapper.Map<VillaDTO>(villa)); 
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddVilla(VillaCreateDTO createDTO)
        {
            if(createDTO == null)
                return BadRequest();

            // custome validation
            if(await _db.Villas.FirstOrDefaultAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Name Must Be Unique!");
                return BadRequest(ModelState);
            }

            /// Manual Mapping
            //Villa model = new Villa()
            //{
            //    Name = createDTO.Name,
            //    Details = createDTO.Details,
            //    Rate = createDTO.Rate,
            //    Sqft = createDTO.Sqft,
            //    Occupancy = createDTO.Occupancy,
            //    ImageUrl = createDTO.ImageUrl,
            //    Amenity = createDTO.Amenity,
            //};

            Villa model = _mapper.Map<Villa>(createDTO);

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction("GetVilla", new {id = model.Id}, model);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if(id <= 0)
                return BadRequest();

            Villa villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if(villa == null) 
                return NotFound();

            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVilla([FromBody] VillaUpdateDTO updateDTO)
        {
            if(updateDTO == null || updateDTO.Id <= 0)
                return BadRequest();

            var model = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == updateDTO.Id);

            if (model == null)
                return NotFound();

            // Manually update only the modified fields
            model = _mapper.Map(updateDTO, model); // Apply changes without overwriting existing object
            model.UpdatedDate = DateTime.Now; // // Update only UpdatedDate

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateCertainField(int id, JsonPatchDocument<VillaUpdateDTO> VillaFromRequest)
        {
            if (id <= 0 || VillaFromRequest == null)
                return BadRequest();

            var villaFromDb = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();

            // Map existing data to DTO
            VillaUpdateDTO updateDTO = _mapper.Map<VillaUpdateDTO>(villaFromDb);

            // Apply patch request
            VillaFromRequest.ApplyTo(updateDTO, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Manually update only the modified fields
            _mapper.Map(updateDTO, villaFromDb); // Apply changes without overwriting existing object

            villaFromDb.UpdatedDate = DateTime.Now; // Update only UpdatedDate

            await _db.SaveChangesAsync();

            return NoContent();
        }


    }
}
