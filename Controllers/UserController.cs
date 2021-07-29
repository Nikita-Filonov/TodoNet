using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserContext _context;
        public static string FrameworkDescription { get; }

        public UserController(UserContext context)
        {
            _context = context;
        }

        // GET: TodoItems
        [HttpPost("/api/v1/users")]
        public IActionResult Post([FromBody] User user)
        {

            User dbUser = new User { 
                Username = user.Username, 
                Email = user.Email, 
                Password = user.Password 
            };

            _context.Users.AddRange(dbUser);

            try
            {
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }
            
            return Ok(dbUser);
        }

        [HttpGet("/api/v1/users/{userId}")]
        public IActionResult GetUser(int userId)
        {
            var dbUser = _context.Users.Find(userId);
            
            if (dbUser == null)
            {
                return NotFound();
            }
            return Ok(dbUser);
        }

        [HttpDelete("/api/v1/users/{userId}")]
        public NoContentResult DeleteUser(int userId)
        {
            User user = new User() { Id = userId };
            _context.Users.Remove(user);
            _context.SaveChanges();
            return new NoContentResult();
        }

        [HttpPatch("/api/v1/users/{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] JsonPatchDocument<User> patchDoc)
        {
            var dbUser = _context.Users.FirstOrDefault(user => user.Id == userId);

            patchDoc.ApplyTo(dbUser, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Update(dbUser);
           
            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
              return NotFound();
            }

            return Ok(dbUser);
        }
    }
}
