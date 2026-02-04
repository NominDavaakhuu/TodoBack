using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public T Data { get; set; }
        public static ApiResponse<T> Ok(T data) => new ApiResponse<T> { Success = true, Data = data };
        public static ApiResponse<T> Fail(string error) => new ApiResponse<T> { Success = false, Error = error };
    }
}
