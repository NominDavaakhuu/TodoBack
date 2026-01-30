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
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(int userId);
        Task  AddAsync(TodoItem item);
        Task UpdateAsync(TodoItem item);
        Task<bool> DeleteAsync(int id);
        Task<int> DeleteByUserIdAsync(int userId);
    }
}
