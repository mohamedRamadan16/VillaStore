using MagicVilla.Models;
using MagicVilla.Models.DTOs;

namespace MagicVilla.Repos.IRepo
{
    public interface IUserRepository
    {
        Task<bool> IsUniqueUser(string UserName);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
    }
}
