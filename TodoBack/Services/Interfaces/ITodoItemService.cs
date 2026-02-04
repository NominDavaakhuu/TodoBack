using Shared.DTOs;
using Shared.DTOs.TodoItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoBack.DTOs;
using TodoBack.Models;

namespace TodoBack.Services.Interfaces
{
    public interface ITodoItemService
    {
        Task<ApiResponse<TodoDto>> GetByIdAsync(long id, long userId);
        Task<ApiResponse<List<TodoDto>>> GetByUserIdAsync(long userId);
        Task<ApiResponse<TodoDto>> CreateAsync(long userId, TodoCreateDto dto);
        Task<ApiResponse<TodoDto>> UpdateAsync(long userId, long id, UpdateTodoDto dto);
        Task<ApiResponse<TodoDto>> DeleteAsync(long id, long userId);
        Task<ApiResponse<TodoDto>> MarkAsCompletedAsync(long id, long userId);
        Task<ApiResponse<int>> DeleteByUserIdAsync(long userId);
    }
}
