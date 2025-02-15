﻿using MagicVilla.Models;
using MagicVilla.Models.DTOs;
using MagicVilla.Repos.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla.Controllers
{
    [Route("api/v{version:apiVersion}/Users")]
    [ApiController]
    //[ApiVersion("1.0")]
    [ApiVersionNeutral]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly APIResponse _response;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _response = new APIResponse();
        }

        [HttpPost("Register")]
        public async Task<ActionResult<APIResponse>> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            if (registerationRequestDTO == null)
            {
                _response.isSuccess = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return _response;
            }

            // check username exists or not
            if (await _userRepository.IsUniqueUser(registerationRequestDTO.UserName))
            {
                try
                {
                    var user = await _userRepository.Register(registerationRequestDTO);
                    _response.isSuccess = true;
                    _response.statusCode = HttpStatusCode.Created;
                    _response.Result = user;

                }
                catch (Exception ex)
                {
                    _response.isSuccess = false;
                    _response.Errors = new List<string>() { ex.ToString() };
                }
            }
            else
            {
                _response.isSuccess = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                _response.Errors = new List<string>() { "Username already exists :(" };
            }

            return _response;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<APIResponse>> Login(LoginRequestDTO loginRequestDTO)
        {
            if (loginRequestDTO == null)
            {
                _response.isSuccess = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return _response;
            }
            try
            {
                var LoginResponse = await _userRepository.Login(loginRequestDTO);
                if (LoginResponse.User == null || LoginResponse.Token == "")
                {
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string>() { "Invalid Login Attempt" };
                }
                else
                {
                    _response.isSuccess = true;
                    _response.statusCode = HttpStatusCode.OK;
                    _response.Result = LoginResponse;

                }
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.Errors = new List<string>() { ex.ToString() };
            }

            return _response;
        }


    }
}
