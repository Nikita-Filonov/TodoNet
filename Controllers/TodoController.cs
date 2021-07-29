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
    public class TodoController : Controller
    {
        private readonly TodoContext _context;
        public static string FrameworkDescription { get; }

        public TodoController(TodoContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpGet("/api/v1/tasks")]
        public JsonResult GetTodoItems()
        {
            var todoItems = _context.TodoItems
                .Where(todoItem => todoItem.UserId == Int64.Parse(User.Identity.Name));
            return new JsonResult(todoItems);
        }

        [Authorize]
        [HttpPost("/api/v1/tasks")]
        public IActionResult CreateTodoItem([FromBody] TodoItem todoItem)
        {
            TodoItem dbTodoItem = new TodoItem
            {
                Title = todoItem.Title,
                Tag = todoItem.Tag,
                UserId = (int)Int64.Parse(User.Identity.Name)
            };

            _context.TodoItems.AddRange(dbTodoItem);
            _context.SaveChanges();
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }

            return Ok(dbTodoItem);
        }

        [Authorize]
        [HttpGet("/api/v1/tasks/{taskId}")]
        public IActionResult GetTodoItem(int taskId)
        {
            var dbTodoItem = _context.TodoItems.Find(taskId);

            if (dbTodoItem == null)
            {
                return NotFound();
            }
            return Ok(dbTodoItem);
        }

        [Authorize]
        [HttpDelete("/api/v1/tasks/{taskId}")]
        public IActionResult DeleteTodoItem(int taskId)
        {
            TodoItem todoItem = new TodoItem() { Id = taskId };
         
            try
            {
                _context.TodoItems.Remove(todoItem);
                _context.SaveChanges();
            }
            catch
            {
                return NotFound();
            }
            
            return new NoContentResult();
        }

        [Authorize]
        [HttpPatch("/api/v1/tasks/{taskId}")]
        public IActionResult UpdateTodoItem(int taskId, [FromBody] JsonPatchDocument<TodoItem> patchDoc)
        {
            var dbTodoItem = _context.TodoItems.FirstOrDefault(user => user.Id == taskId);

            patchDoc.ApplyTo(dbTodoItem, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Update(dbTodoItem);

            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return Ok(dbTodoItem);
        }
    }
}

