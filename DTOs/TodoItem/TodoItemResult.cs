using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoBack.DTOs.TodoItem
{
    public class TodoItemResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public long Id { get; set; }
        public string Title { get; set; }
        public long? CategoryId { get; set; }
    }
}