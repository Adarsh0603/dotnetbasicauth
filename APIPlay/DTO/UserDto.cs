using APIPlay.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIPlay.DTO
{
    public class UserDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; }= string.Empty;
        [Required]
        public UserRole Role { get; set; }
    }
}
