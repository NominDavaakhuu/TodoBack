using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.TodoItem
{
    public class UpdateTodoDto
    {   
        public string Title { get; set; }
        public long? CategoryId { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
