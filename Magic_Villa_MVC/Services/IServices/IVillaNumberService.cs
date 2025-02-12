using Magic_Villa_MVC.Models.DTOs;

namespace Magic_Villa_MVC.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>(string token);
        Task<T> GetAsync<T>(int id, string token);
        Task<T> CreateAsync<T>(VillaNumberCreatedDTO dto, string token);
        Task<T> UpdateAsync<T>(VillaNumberUpdatedDTO dto, string token);
        Task<T> DeleteAsync<T>(int id, string token);
    }
}
