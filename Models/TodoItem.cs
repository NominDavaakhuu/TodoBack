using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TodoBack.Models
{
    public class TodoItem
    {
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; } = string.Empty;
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        //soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        //optional category
        public long? CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}