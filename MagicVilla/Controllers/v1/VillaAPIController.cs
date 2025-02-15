﻿using AutoMapper;
using MagicVilla.Data;
using MagicVilla.Loggings;
using MagicVilla.Models;
using MagicVilla.Models.DTOs;
using MagicVilla.Repos.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla.v1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiVersion("1.0")]
    public class VillaAPIController:ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;

        //private readonly ILogger<VillaAPIController> _logger;
        /// For A Custom Logging
        private readonly ILogging _logger;

        public VillaAPIController(ILogging logger, IVillaRepository villaRepository, IMapper mapper)
        {
            _response = new APIResponse();
            _villaRepository = villaRepository;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        //[ResponseCache(Duration = 30)]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetAllVillas([FromQuery(Name = "OccupancyFilter")] int? occupancy, [FromQuery] string? name, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                _logger.Log("Getting All Villas :D", "");

                /// adding basic filter
                /// calling pagination : https://localhost:7001/api/v1/villaapi?pagesize=5&pagenumber=1
                IEnumerable<Villa> villaList;
                if (occupancy > 0)
                    villaList = await _villaRepository.GetAllAsync(v => v.Occupancy == occupancy, pageSize:pageSize, pageNumber:pageNumber);
                else
                    villaList = await _villaRepository.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

                /// search for filla based on the name
                /// https://localhost:7001/api/v1/villaapi?name=pool&occupancyfilter=4
                if (!string.IsNullOrEmpty(name))
                    villaList = villaList.Where(v => v.Name.ToLower().Contains(name.ToLower()));

                // adding pagination to the response headers
                Pagination pagination = new Pagination() { PageNumber = pageNumber, PageSize = pageSize};
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _response.statusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
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
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {

                if (id <= 0)
                {
                    _logger.Log($"Error Getting Villa With ID : {id}", "Error");
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.isSuccess = false;
                    _response.Errors = new List<string>() { "Invalid Villa Id" };
                    return BadRequest(_response);
                }
                var villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.isSuccess = false;
                    _response.Errors = new List<string>() { "Invalid Villa Id" };
                    return NotFound(_response);
                }

                _response.statusCode = HttpStatusCode.OK;
                _response.Result = _mapper.Map<VillaDTO>(villa);

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

        [Authorize(Roles = "admin")]

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> AddVilla(VillaCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }

                // custome validation
                if (await _villaRepository.GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa Name Must Be Unique!");
                    _response.statusCode = HttpStatusCode.BadRequest;
                    //return BadRequest(_response);
                    return BadRequest(ModelState);
                }

                Villa villa = _mapper.Map<Villa>(createDTO);
                await _villaRepository.CreateAsync(villa);


                _response.statusCode = HttpStatusCode.Created;
                _response.Result = villa;

                return CreatedAtAction("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.statusCode = HttpStatusCode.InternalServerError;
                _response.isSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [Authorize(Roles = "admin")]

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Villa villa = await _villaRepository.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _villaRepository.RemoveAsync(villa);

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

        [Authorize(Roles = "admin")]

        [HttpPut]
        public async Task<ActionResult<APIResponse>> UpdateVilla([FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.Id <= 0)
                {
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var model = await _villaRepository.GetAsync(v => v.Id == updateDTO.Id, tracked: false);

                if (model == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                // Manually update only the modified fields
                model = _mapper.Map(updateDTO, model); // Apply changes without overwriting existing object

                await _villaRepository.UpdateAsync(model);

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

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateCertainField(int id, JsonPatchDocument<VillaUpdateDTO> VillaFromRequest)
        {
            if (id <= 0 || VillaFromRequest == null)
                return BadRequest();

            var villaFromDb = await _villaRepository.GetAsync(v => v.Id == id);
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

            await _villaRepository.SaveAsync();
            return NoContent();
        }


    }
}
