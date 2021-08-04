using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class TodoList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public List<User> Users { get; set; } = new List<User>();
        
        public List<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}
