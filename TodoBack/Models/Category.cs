using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TodoBack.Models
{
    public class Category
    {
        public long Id { get; set; }

        [Required, MaxLength(40)]
        public string Name { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<TodoItem> TodoItems { get; set; } = new HashSet<TodoItem>();
    }
}