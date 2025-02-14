using AutoMapper;
using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Models.DTOs;
using MagicVilla.Repos.IRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        string secretKey;
        public UserRepository(ApplicationDbContext db, IMapper mapper, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._db = db;
            this._mapper = mapper;
            _userManager = userManager;
            secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<bool> IsUniqueUser(string UserName)
        {
            var User = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName.ToLower() == UserName.ToLower());
            return User == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (user == null || isValidPassword == false)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null

                };
            }
               
            var roles = await _userManager.GetRolesAsync(user);
            // user is valid then generate JWT
           
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user)

            };
            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser User = new ApplicationUser();

            if(registerationRequestDTO != null)
            {
                User = _mapper.Map<ApplicationUser>(registerationRequestDTO);
                User.Email = registerationRequestDTO.UserName;
                var result = await _userManager.CreateAsync(User, registerationRequestDTO.Password);
                if (!result.Succeeded)
                {
                    return new UserDTO() { ID = "", Name = "", UserName = "" };
                }

                // create role if not exists
                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("customer"));

                }
                await _userManager.AddToRoleAsync(User, "admin");
            }

            return _mapper.Map<UserDTO>(User);
        }
    }
}
