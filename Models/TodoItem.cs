﻿using System;
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
        public bool IsComplete { get; set; } = false;
        //[ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
