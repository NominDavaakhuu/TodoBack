using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
