using APIPlay.Enums;
using System.ComponentModel.DataAnnotations;

namespace APIPlay.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }=string.Empty;

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public string? Email { get; set; }
        public UserRole Role { get; set; }


    }
}
