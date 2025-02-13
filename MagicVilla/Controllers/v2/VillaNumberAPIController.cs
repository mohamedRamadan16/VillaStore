using AutoMapper;
using MagicVilla.Data;
using MagicVilla.Loggings;
using MagicVilla.Models;
using MagicVilla.Models.DTOs;
using MagicVilla.Repos.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla.v2.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiVersion("2.0")]

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
        //[MapToApiVersion("2.0")]
        public IActionResult GetAll()
        {
            return Ok(new List<string>() { "v1", "v2" });
        }

    }
}
