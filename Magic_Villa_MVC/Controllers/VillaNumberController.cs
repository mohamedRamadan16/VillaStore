using AutoMapper;
using Magic_Villa_MVC.Models;
using Magic_Villa_MVC.Models.DTOs;
using Magic_Villa_MVC.Services;
using Magic_Villa_MVC.Services.IServices;
using Magic_Villa_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Magic_Villa_MVC.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService, IMapper mapper)
        {
            _villaNumberService = villaNumberService;
            _villaService = villaService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new List<VillaNumberDTO>();
            var response = await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.isSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> CreateVillaNumber()
        {
            List<VillaDTO> list = new List<VillaDTO>();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.isSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }

            ViewBag.Villas = list;

            return View();
        }

        [Authorize(Roles = "admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreatedDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.isSuccess)
                {
                    TempData["success"] = "Villa number created successfully";
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
            }
            TempData["error"] = "Error encountered.";
            return View(model);
        }

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
        {
            List<VillaDTO> list = new List<VillaDTO>();

            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
            var villas = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.isSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(villas.Result));
                ViewBag.Villas = list;
                return View(_mapper.Map<VillaNumberUpdatedDTO>(model));
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdatedDTO model)
        {
            if (ModelState.IsValid)
            {
                TempData["success"] = "Villa number updated successfully";
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
            }
            TempData["error"] = "Error encountered.";
            return View(model);
        }

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
        {
            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.isSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDTO model)
        {
            var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.isSuccess)
            {
                TempData["success"] = "Villa number deleted successfully";
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            TempData["error"] = "Error encountered.";
            return View(model);
        }
    }
}
