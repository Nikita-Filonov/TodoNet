using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly TodoContext _context;
        public static string FrameworkDescription { get; }

        public UserController(TodoContext context)
        {
            _context = context;
        }

        protected UserPrivateDto UserResponseData(User user)
        {
            return new UserPrivateDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ImportantTodos = user.ImportantTodos,
                ImportantGroups = user.ImportantGroups
            };
        }

        [HttpPost("/api/v1/users")]
        public IActionResult Post([FromBody] User user)
        {
            var isUserExists = _context.Users.Any(u => u.Email == user.Email);
            if (isUserExists)
            {
                return BadRequest(new { 
                  message = $"User with email {user.Email} already exists",
                  level = "danger"
                });
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User dbUser = new()
            { 
                Username = user.Username, 
                Email = user.Email, 
                Password = user.Password 
            };
            _context.Users.AddRange(dbUser);
            _context.SaveChanges();
            return Ok(UserResponseData(dbUser));
        }

        [Authorize]
        [HttpGet("/api/v1/users/{userId}")]
        public IActionResult GetUser(int userId)
        {
            var dbUser = _context.Users.Find(userId);
            
            if (dbUser == null)
            {
                return NotFound();
            }
            return Ok(UserResponseData(dbUser));
        }

        [Authorize]
        [HttpDelete("/api/v1/users/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User user = new() { Id = userId };

            if (userId != Int64.Parse(User.Identity.Name))
            {
                return Utils.Errors.ForbiddenError();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [Authorize]
        [HttpPatch("/api/v1/users/{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            var dbUser = _context.Users.FirstOrDefault(user => user.Id == userId);

            if (userId != Int64.Parse(User.Identity.Name))
            {
                return Utils.Errors.ForbiddenError();
            }

            patchDoc.ApplyTo(dbUser, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Update(dbUser);
           
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
              return NotFound();
            }

            return Ok(UserResponseData(dbUser));
        }
    }
}
