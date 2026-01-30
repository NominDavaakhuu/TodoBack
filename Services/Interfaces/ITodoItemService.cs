using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoBack.DTOs;
using TodoBack.DTOs.TodoItem;
using TodoBack.Models;

namespace TodoBack.Services.Interfaces
{
    public interface ITodoItemService
    {
        Task<TodoItemResult> GetByIdAsync(int id, int userId);
        Task<IEnumerable<TodoItemResult>> GetByUserIdAsync(int userId);
        Task<TodoItemResult> CreateAsync(int userId, TodoCreateDto dto);
        Task<TodoItemResult> UpdateAsync(int userId, long id, UpdateTodoDto dto);
        Task<TodoItemResult> DeleteAsync(int id, int userId);
        Task<int> DeleteByUserIdAsync(int userId);
        Task<TodoItemResult> MarkAsCompletedAsync(int id, int userId);
    }
}
