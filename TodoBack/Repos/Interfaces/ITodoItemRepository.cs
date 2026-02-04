using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoBack.DTOs;
using TodoBack.Models;

namespace TodoBack.Repos.Interfaces
{
    public interface ITodoItemRepository
    {
        Task<TodoItem> GetByIdAsync(long id);
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(long userId);
        Task  AddAsync(TodoItem item);
        Task UpdateAsync(TodoItem item);
        Task<bool> DeleteAsync(long id);
        Task<int> DeleteByUserIdAsync(long userId);
    }
}
