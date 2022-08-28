using AuthenticationApi.Models;

namespace AuthenticationApi.IService
{
    public interface IUserService
    {
        User Register(UserDto user);
        string Login(string password, User suser);
    }
}
