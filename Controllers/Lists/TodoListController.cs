﻿using System;
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
        [HttpGet("/api/v1/lists")]
        public IActionResult GetTodoLists()
        {
            var dbTodoLists = _context.TodoLists
                .Where(l => l.Users.All(u => u.Id == (int)Int64.Parse(User.Identity.Name)))
                .Select(list => new { 
                    list.Id,
                    list.Title,
                    Users = list.Users
                    .Select(u => new { u.Id, u.Username, u.Email }),
                    list.TodoItems,
                    list.Created,
                    list.Background
                })
                .ToList();

            return Ok(dbTodoLists);
        }

        [Authorize]
        [HttpGet("/api/v1/lists/{listId}")]
        public IActionResult GetTodoList(int listId)
        {
            var isUserInList = _context.TodoLists
                .Where(l => l.Users.All(u => u.Id == (int)Int64.Parse(User.Identity.Name)))
                .FirstOrDefault(l => l.Id == listId);

            if (isUserInList == null)
            {
                return Utils.Errors.ForbiddenError();
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
                    list.Created,
                    list.Background
                })
                .FirstOrDefault();

            return Ok(todoList);
        }


        [Authorize]
        [HttpPost("/api/v1/lists")]
        public IActionResult CreateTodoList([FromBody] TodoList todoList)
        {
            TodoList dbTodoList = new()
            {
                Title = todoList.Title,
                Background = todoList.Background
            };

            _context.TodoLists.AddRange(dbTodoList);
            try
            {
                var dbUser = _context.Users.Find((int)Int64.Parse(User.Identity.Name));
                dbTodoList.Users.Add(dbUser);
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }
            return Ok(new {
                dbTodoList.Id,
                dbTodoList.Title,
                Users = dbTodoList.Users
                .Select(u => new { u.Id, u.Username, u.Email }),
                dbTodoList.TodoItems,
                dbTodoList.Created,
                dbTodoList.Background
            });
        }

        [Authorize]
        [HttpDelete("/api/v1/lists/{listId}")]
        public IActionResult DeleteTodoList(int listId)
        {
            var dbTodoList = _context.TodoLists
                .Where(l => l.Users.All(u => u.Id == (int)Int64.Parse(User.Identity.Name)))
                .FirstOrDefault(l => l.Id == listId);

            if (dbTodoList == null)
            {
                return Utils.Errors.ForbiddenError();
            }

            try
            {
                _context.TodoLists.Remove(dbTodoList);
                _context.SaveChanges();
            }
            catch
            {
                return NotFound();
            }

            return new NoContentResult();
        }

        [Authorize]
        [HttpPatch("/api/v1/lists/{listId}")]
        public IActionResult UpdateTodoList(int listId, [FromBody] JsonPatchDocument<TodoList> patchDoc)
        {
            var dbTodoList = _context.TodoLists
                .Where(l => l.Users.All(u => u.Id == (int)Int64.Parse(User.Identity.Name)))
                .FirstOrDefault(l => l.Id == listId);

            if (dbTodoList == null)
            {
                return Utils.Errors.ForbiddenError();
            }

            patchDoc.ApplyTo(dbTodoList, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Update(dbTodoList);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
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
                    list.Created,
                    list.Background
                })
                .FirstOrDefault();
            return Ok(todoList);
        }
    }
}

