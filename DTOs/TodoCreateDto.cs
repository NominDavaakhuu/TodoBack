using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoBack.DTOs
{
    public class TodoCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public long? CategoryId { get; set; }
    }
}