using Newtonsoft.Json;
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

        [Required]
        [MaxLength(70)]
        public string Password { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [JsonIgnore]
        public List<TodoList> Users { get; set; } = new List<TodoList>();

        public int[] ImportantTodos { get; set; } = System.Array.Empty<int>();

        public int[] ImportantGroups { get; set; } = System.Array.Empty<int>();
        //  public ICollection<TodoItem> TodoItems { get; set; }
    }

    public class UserPrivateDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int[] ImportantTodos { get; set; }
        public int[] ImportantGroups { get; set; }
    }

    public class UserPublicDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
