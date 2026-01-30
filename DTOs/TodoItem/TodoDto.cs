using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TodoBack.Models;

namespace TodoBack.DTOs
{
    public class TodoDto
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public long? CategoryId { get; set; }
        public int UserId { get; set; }
    }
}