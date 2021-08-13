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
        [HttpGet("/api/v1/lists/{listId}/tasks")]
        public JsonResult GetTodoItems(int listId)
        {
            var todoList = _context.TodoLists
                .Where(list => list.Id == listId)
                .Select(list => list.TodoItems)
                .ToList();

            return new JsonResult(todoList[0]);
        }

        [Authorize]
        [HttpPost("/api/v1/lists/{listId}/tasks")]
        public IActionResult CreateTodoItem(int listId, [FromBody] TodoItem todoItem)
        {
            TodoItem dbTodoItem = new ()
            {
                Title = todoItem.Title,
                Tag = todoItem.Tag,
            };

            _context.TodoItems.AddRange(dbTodoItem);
            try
            {
                var dbTodoList = _context.TodoLists.Find(listId);
                dbTodoList.TodoItems.Add(dbTodoItem);
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }

            return Ok(new {
                dbTodoItem.Id,
                dbTodoItem.Title,
                dbTodoItem.Tag,
                dbTodoItem.IsComplete,
                dbTodoItem.Created
            });
        }

        [Authorize]
        [HttpDelete("/api/v1/lists/{listId}/tasks/{taskId}")]
        public IActionResult DeleteTodoItem(int taskId)
        {          
            TodoItem todoItem = new () { Id = taskId };
         
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
        [HttpPatch("/api/v1/lists/{listId}/tasks/{taskId}")]
        public IActionResult UpdateTodoItem(int taskId, [FromBody] JsonPatchDocument<TodoItem> patchDoc)
        {
            var dbTodoItem = _context.TodoItems.FirstOrDefault(todoItem => todoItem.Id == taskId);

            patchDoc.ApplyTo(dbTodoItem, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Update(dbTodoItem);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return Ok(dbTodoItem);
        }
    }
}

