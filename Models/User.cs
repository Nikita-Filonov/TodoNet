using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public List<TodoList> Users { get; set; } = new List<TodoList>();


        //  public ICollection<TodoItem> TodoItems { get; set; }
    }
}
