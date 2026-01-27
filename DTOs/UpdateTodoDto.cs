using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoBack.DTOs
{
    public class UpdateTodoDto
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public long? CategoryId { get; set; }
    }
}