using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TodoBack.Models
{
    public class User
    {
        public long Id { get; set; }

        [Required, MaxLength(40)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(40)]
        [Index("IX_User_UsernameNormalized", IsUnique= true)]
        public string UsernameNormalized { get; set; } = string.Empty;

        [Required, MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(256)]
        [Index("IX_User_EmailNormalized", IsUnique = true)]
        public string EmailNormalized { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public virtual ICollection<TodoItem> TodoItems { get; set; } = new HashSet<TodoItem>();
    }
}