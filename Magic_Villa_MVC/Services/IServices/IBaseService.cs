using Magic_Villa_MVC.Models;

namespace Magic_Villa_MVC.Services.IServices
{
    public interface IBaseService
    {
        APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
