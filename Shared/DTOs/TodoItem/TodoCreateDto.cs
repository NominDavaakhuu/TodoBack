using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.TodoItem
{
    public class TodoCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public long? CategoryId { get; set; }
    }
}
