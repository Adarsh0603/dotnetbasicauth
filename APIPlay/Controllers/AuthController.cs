using APIPlay.Data;
using APIPlay.DTO;
using APIPlay.Enums;
using APIPlay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace APIPlay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase

    {
        private readonly ApplicationDBContext _db;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDBContext db,IConfiguration configuration)
        {
            _db= db;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<string>> Register(UserDto userdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            byte[] passwordHash;
            byte[] passwordSalt;
            GenerateHash(userdto.Password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Username = userdto.Username,
                PasswordHash =passwordHash,
                PasswordSalt= passwordSalt,
                Role = userdto.Role,
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            return CreateToken(user);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<string>> Login(UserDto userdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user =  _db.Users.FirstOrDefault(u => u.Username == userdto.Username);
            if (user == null)
            {
                return NotFound("User with this username not found.");
            }
            if (!VerifyPasswordHash(user,userdto.Password))
            {
                return BadRequest("Invalid Password");
            }
            
            return CreateToken(user);


        }


        private void GenerateHash(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using(var hmac= new HMACSHA512())
            {
                 passwordSalt = hmac.Key;
                 passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
        private bool VerifyPasswordHash(User user,string password)
        {
            using (var hmac= new HMACSHA512(user.PasswordSalt))
            {
                var currentPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return user.PasswordHash.SequenceEqual(currentPasswordHash);
            }
        }
        private string CreateToken(User user) {
            List<Claim> claims = new List<Claim>
                {
                    new Claim("Username",user.Username),
                    new Claim(ClaimTypes.Role,user.Role.ToString()),
                };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(claims:claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }

        [HttpGet, Route("data")]
        public ActionResult<string> GetData()
        {
            return "Hello";
        }

        [HttpGet, Route("dataLocked"), Authorize(Roles ="ADMIN")]
        public ActionResult<string> GetDataLocked()
        {
            return "Hello Privacy";
        }
    }
}
