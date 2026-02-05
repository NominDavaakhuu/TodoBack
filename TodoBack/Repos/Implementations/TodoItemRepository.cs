using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TodoBack.Data;
using TodoBack.DTOs;
using TodoBack.Models;
using TodoBack.Repos.Interfaces;

namespace TodoBack.Repos.Implementations
{
    public class TodoItemRepository: ITodoItemRepository
    {
        private readonly TodoDbContext _db;

        public TodoItemRepository(TodoDbContext db) => _db = db;

        public Task<TodoItem> GetByIdAsync(long id)
        {
            return _db.TodoItems.SingleOrDefaultAsync(t => t.Id == id);
            //return await _db.TodoItems.FindAsync(id);
        }

        public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(long userId)
        {
            return await _db.TodoItems
                        .Where(t => t.UserId == userId && !t.IsDeleted)
                        .OrderByDescending(t => t.CreatedAt)
                        .ToListAsync();
        }

        public async Task AddAsync(TodoItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _db.TodoItems.Add(item);
            await _db.SaveChangesAsync();

        }

        public async Task UpdateAsync(TodoItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var itemToDelete = _db.TodoItems.SingleOrDefault(t => t.Id == id);
            if (itemToDelete == null) return false;
            _db.Entry(itemToDelete).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<int> DeleteByUserIdAsync(long userId)
        {
            var items = await _db.TodoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();

            if (items.Count == 0)
                return 0;

            foreach (var item in items)
            {
                item.IsDeleted = true;
            }

            await _db.SaveChangesAsync();
            return items.Count;
        }
    }
}