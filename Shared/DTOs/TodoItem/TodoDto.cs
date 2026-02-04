using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.TodoItem
{
    public class TodoDto
    {
        public long? Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public bool? IsCompleted { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public long? CategoryId { get; set; }
    }
}
