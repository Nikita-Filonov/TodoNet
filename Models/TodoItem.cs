
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class TodoItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string Tag { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonIgnore]
        public List<TodoList> TodoItems { get; set; } = new List<TodoList>();
    }
}
