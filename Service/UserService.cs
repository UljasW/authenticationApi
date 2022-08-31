using AuthenticationApi.IService;
using AuthenticationApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthenticationApi.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Login(string password, User user)
        {
            if (Identify(password,user))
            {
                return CreateToken(user);
            }
            else throw new Exception("Wrong Password");
        }

        public User Register(UserDto userDto)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            User user= new User()
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                PasswordSalt = salt,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password, salt)
            };

            return user;
        }

        public bool DeleteUser(string password, User user)
        {
            bool del = false;
            try
            {
                del = Identify(password, user);
            }
            catch (Exception)
            {
                throw;
            }
            return del;
        }

        private bool Identify(string password, User user)
        {
            string _newHash = BCrypt.Net.BCrypt.HashPassword(password, user.PasswordSalt);
            return _newHash.SequenceEqual(user.PasswordHash);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims : claims,
                expires : DateTime.Now.AddDays(1),
                signingCredentials : creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
                     
    }
}
