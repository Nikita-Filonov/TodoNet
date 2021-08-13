using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListUsersController : Controller
    {
        private readonly TodoContext _context;
        public static string FrameworkDescription { get; }

        public TodoListUsersController(TodoContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("/api/v1/lists/{listId}/users")]
        public IActionResult GetTodoListUsers(int listId)
        {
            var todoList = _context.TodoLists
                .Where(l => l.Id == listId)
                .Select(list => new
                {
                    Users = list.Users
                    .Select(u => new { u.Id, u.Username, u.Email }),
                })
                .FirstOrDefault();
            return Ok(todoList.Users);
        }


        [Authorize]
        [HttpPost("/api/v1/lists/{listId}/users")]
        public IActionResult CreateTodoListUser(int listId, [FromBody] UserPublicDto user)
        {
            var isUsernameValid = !string.IsNullOrWhiteSpace(user.Username);
            var isEmailValid = !string.IsNullOrWhiteSpace(user.Email);
            var dbUser = _context.Users
                .Where(u =>
                    (isUsernameValid && u.Username == user.Username ||
                    (isEmailValid && u.Email == user.Email)
                ))
                .FirstOrDefault();
                

            if (dbUser == null)
            {
                return NotFound();
            }
            try
            {
                var dbTodoList = _context.TodoLists.Find(listId);
                dbTodoList.Users.Add(dbUser);
                _context.SaveChanges();
            } 
            catch
            {
                return BadRequest(new { 
                   message = "List already contains such user",
                   level = "warning"
                });
            }

            var todoList = _context.TodoLists
                .Where(l => l.Id == listId)
                .Select(list => new
                {
                    list.Id,
                    list.Title,
                    Users = list.Users
                    .Select(u => new { u.Id, u.Username, u.Email }),
                    list.TodoItems,
                    list.Created
                })
                .FirstOrDefault();
            return Ok(todoList);
        }

        [Authorize]
        [HttpDelete("/api/v1/lists/{listId}/users/{userId}")]
        public IActionResult DeleteTodoList(int listId, int userId)
        {
            TodoList todoList = _context.TodoLists
                .Include(l => l.Users)
                .FirstOrDefault(l => l.Id == listId);
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            try
            {
                todoList.Users.Remove(user);
                _context.SaveChanges();
            }
            catch
            {
                return NotFound();
            }

            return new NoContentResult();
        }
    }
}
