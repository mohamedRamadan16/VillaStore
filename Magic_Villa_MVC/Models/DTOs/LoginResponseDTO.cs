namespace Magic_Villa_MVC.Models.DTOs
{
    public class LoginResponseDTO
    {
        public UserDTO LocalUser { get; set; }
        public string Token { get; set; }

    }
}
