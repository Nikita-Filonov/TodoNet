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
        public JsonResult Get()
        {
            var todoItems = _context.TodoItems
                .Where(todoItem => todoItem.UserId == Int64.Parse(User.Identity.Name));
            return new JsonResult(todoItems);
        }

        [HttpPost("/api/v1/tasks")]
        public IActionResult Post([FromBody] TodoItem todoItem)
        {
            var dbUser = _context.TodoItems.Find(todoItem.UserId);
            TodoItem dbTodoItem = new TodoItem
            {
                Title = todoItem.Title,
                Tag = todoItem.Tag,
                UserId = todoItem.UserId
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

        [HttpDelete("/api/v1/tasks/{taskId}")]
        public NoContentResult DeleteTodoItem(int taskId)
        {
            TodoItem todoItem = new TodoItem() { Id = taskId };
            _context.TodoItems.Remove(todoItem);
            _context.SaveChanges();
            return new NoContentResult();
        }

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

