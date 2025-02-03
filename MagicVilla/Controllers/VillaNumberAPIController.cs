using AutoMapper;
using MagicVilla.Data;
using MagicVilla.Loggings;
using MagicVilla.Models;
using MagicVilla.Models.DTOs;
using MagicVilla.Repos.IRepo;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaNumberAPIController:ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;

        public VillaNumberAPIController(ILogging logger, IVillaNumberRepository villaNumberRepository, IVillaRepository villaRepository, IMapper mapper)
        {
            _response = new APIResponse();
            _villaNumberRepository = villaNumberRepository;
            _villaRepository = villaRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetAllVillasNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumberList = await _villaNumberRepository.GetAllAsync();
                _response.statusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.isSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200, Type = typeof(VillaDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {

                if (id <= 0)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villaNum = await _villaNumberRepository.GetAsync(v => v.VillaNo == id);
                if (villaNum == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.statusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNum);

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.statusCode = HttpStatusCode.InternalServerError;
                _response.isSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }

            return _response;

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> AddVillaNumber(VillaNumberCreatedDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }

                if(await _villaRepository.GetAsync(v => v.Id == createDTO.VillaId) == null)
                {
                    ModelState.AddModelError("Custom Error", "Invalid Villa Id");
                    return BadRequest(ModelState);
                }

                VillaNumber villa = _mapper.Map<VillaNumber>(createDTO);
                await _villaNumberRepository.CreateAsync(villa);


                _response.statusCode = HttpStatusCode.Created;
                _response.Result = createDTO;

                return CreatedAtAction("GetVillaNumber", new { id = villa.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.statusCode = HttpStatusCode.InternalServerError;
                _response.isSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                VillaNumber villa = await _villaNumberRepository.GetAsync(v => v.VillaNo == id);
                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _villaNumberRepository.RemoveAsync(villa);

                _response.statusCode = HttpStatusCode.NoContent;
                _response.isSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.statusCode = HttpStatusCode.InternalServerError;
                _response.isSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPut]
        public async Task<ActionResult<APIResponse>> UpdateVilla([FromBody] VillaNumberUpdatedDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.VillaNo <= 0)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _villaRepository.GetAsync(v => v.Id == updateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("Custom Error", "Invalid Villa Id");
                    return BadRequest(ModelState);
                }

                var model = await _villaNumberRepository.GetAsync(v => v.VillaNo == updateDTO.VillaNo, tracked: false);

                if (model == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // Manually update only the modified fields
                model = _mapper.Map(updateDTO, model); // Apply changes without overwriting existing object

                await _villaNumberRepository.UpdateAsync(model);

                _response.statusCode = HttpStatusCode.NoContent;
                _response.isSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.statusCode = HttpStatusCode.InternalServerError;
                _response.isSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }

            return _response;
        }

    }
}
