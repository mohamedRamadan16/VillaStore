using Magic_Villa_MVC.Models;
using Magic_Villa_MVC.Models.DTOs;
using Magic_Villa_MVC.Services.IServices;
using Magic_Villa_Utility;

namespace Magic_Villa_MVC.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");

        }

        public Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = villaUrl + "/api/Users/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = villaUrl + "/api/Users/register"
            });
        }
    }
}
