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
    public class TodoListController : Controller
    {
        private readonly TodoContext _context;
        public static string FrameworkDescription { get; }

        public TodoListController(TodoContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpPost("/api/v1/lists")]
        public IActionResult CreateTodoLIst([FromBody] TodoList todoList)
        {
            TodoList dbTodoList = new()
            {
                Title = todoList.Title,
            };

            _context.TodoLists.AddRange(dbTodoList);
            try
            {
                _context.SaveChanges();
                var dbUser = _context.Users.Find((int)Int64.Parse(User.Identity.Name));
                dbTodoList.Users.Add(dbUser);
            }
            catch
            {
                return BadRequest();
            }

            return Ok(dbTodoList);
        }

    }
}

