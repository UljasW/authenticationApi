using AuthenticationApi.IService;
using AuthenticationApi.Models;

namespace AuthenticationApi.Service
{
    public class UserService : IUserService
    {
        public string Login(string password, User user)
        {
            string _newHash = BCrypt.Net.BCrypt.HashPassword(password, user.PasswordSalt);

            if (_newHash.SequenceEqual(user.PasswordHash))
            {
                return "Crazy token";
            }
            else throw new Exception("Wrong PassWord");


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

        private string CreateToken()
        {
            return "";
        }
                     
    }
}
