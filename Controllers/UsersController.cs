using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthenticationApi.Models;
using AuthenticationApi.Service;
using System.Collections;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        UserService _userService;

        public UsersController(UserContext context, IConfiguration configuration)
        {
            _context = context;
            _userService = new UserService(configuration);
        }

        // GET: api/Users
        [HttpGet("Login")]
        
        public async Task<ActionResult<string>> GetUsers(string UserName, string Password)
        {
            var data = _context.Users;
            var query = from user in data
                       where user.UserName == UserName
                              select user;

            if (query.Count() > 0)
            {
                try
                {
                    return _userService.Login(Password, (User)query.First());
                }
                catch (Exception e)
                {

                    return BadRequest(e.Message);
                }
                
            }
            else return BadRequest("User not found");
        }

        [HttpGet("GetAllUsers"), Authorize(Roles = "admin")]

        public IEnumerable<User> GetAllUsers()
        {
            var data = _context.Users;
            var query = from user in data select user;

            return query;
        }

        [HttpPost("Registrer")]
        public async Task<ActionResult<User>> PostUser(UserDto userDto)
        {
            User user = _userService.Register(userDto);
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
       
            return CreatedAtAction("GetUser", new { id = userDto.UserName }, user);
        }

        // DELETE: api/DataModels/5
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser(string UserName, string Password)
        {
            var data = _context.Users;
            var query = from user in data
                       where user.UserName == UserName
                       select user;

            

            if (query.Count() > 0)
            {
                bool del = _userService.DeleteUser(Password, (User)query.First());
                if (del)
                {
                    var dataModel = await _context.Users.FindAsync(UserName);
                    _context.Users.Remove(dataModel);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest("Wrong password");
                }
            }
            else return BadRequest("User not found");

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.UserName == id)).GetValueOrDefault();
        }
    }
}
