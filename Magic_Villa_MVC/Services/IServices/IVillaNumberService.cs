using Magic_Villa_MVC.Models.DTOs;

namespace Magic_Villa_MVC.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaNumberCreatedDTO dto);
        Task<T> UpdateAsync<T>(VillaNumberUpdatedDTO dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
