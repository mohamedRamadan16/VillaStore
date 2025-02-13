using Magic_Villa_MVC.Models;
using Magic_Villa_MVC.Models.DTOs;
using Magic_Villa_MVC.Services.IServices;
using Magic_Villa_Utility;

namespace Magic_Villa_MVC.Services
{
    public class VillaNumberService : BaseService, IVillaNumberService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;

        public VillaNumberService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> CreateAsync<T>(VillaNumberCreatedDTO dto, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url = villaUrl + "/api/v1/villaNumberAPI", 
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = villaUrl + "/api/v1/villaNumberAPI/" + id,
                Token = token
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + "/api/v1/villaNumberAPI",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = villaUrl + "/api/v1/villaNumberAPI/" + id,
                Token = token
            });
        }

        public Task<T> UpdateAsync<T>(VillaNumberUpdatedDTO dto, string token)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = villaUrl + "/api/v1/villaNumberAPI/",
                Token = token
            });
        }
    }
}
