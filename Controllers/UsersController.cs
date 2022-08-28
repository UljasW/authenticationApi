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

namespace AuthenticationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        UserService _userService = new UserService();

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        
        public async Task<ActionResult<string>> GetUsers(string UserName, string Password)
        {
            var data = _context.Users.AsQueryable();
            var temp = from user in data
                       where user.UserName == UserName
                              select user;

            if (temp != null)
            {
                try
                {
                    return _userService.Login(Password, (User)temp);
                }
                catch (Exception e)
                {

                    return BadRequest(e.Message);
                }
                
            }
            else return BadRequest("User not found");
        }
       

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetUser() { 

            return "";

        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]

     

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
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

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.UserName == id)).GetValueOrDefault();
        }
    }
}
