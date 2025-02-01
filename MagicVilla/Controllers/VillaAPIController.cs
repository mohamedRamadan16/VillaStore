using MagicVilla.Loggings;
using MagicVilla.Models;
using MagicVilla.Models.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaAPIController:ControllerBase
    {
        //private readonly ILogger<VillaAPIController> _logger;
        /// For A Custom Logging
        private readonly ILogging _logger;

        public VillaAPIController(ILogging logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<VillaDTO>> GetAllVillas()
        {
            _logger.Log("Getting All Villas :D", "");
            return Ok(VillaStore.VillaList);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.Log($"Error Getting Villa With ID : {id}", "Error");
                return BadRequest();
            }
            VillaDTO villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
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
            if(VillaStore.VillaList.FirstOrDefault(v => v.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Name Must Be Unique!");
                return BadRequest(ModelState);
            }

            VillaStore.AddVilla(villa);
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

            VillaDTO villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if(villa == null) 
                return NotFound();

            VillaStore.DeleteVilla(villa);
            return NoContent();
        }

        [HttpPut]
        public ActionResult UpdateVilla([FromBody] VillaDTO villaFromRequest)
        {
            if(villaFromRequest == null || villaFromRequest.Id <= 0)
                return BadRequest();

            VillaStore.UpdateVilla(villaFromRequest);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult UpdateCertainField(int id, JsonPatchDocument<VillaDTO> VillaFromRequest)
        {
            if(id <= 0 || VillaFromRequest == null)
                return BadRequest();
            VillaDTO villaFromDb = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();

            VillaFromRequest.ApplyTo(villaFromDb, ModelState);
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }

    }
}
