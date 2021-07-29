using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(255)]
        public string Username { get; set; }
        [MaxLength(70)]
        public string Password { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
