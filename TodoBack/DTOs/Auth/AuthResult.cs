using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoBack.DTOs.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
    }
}