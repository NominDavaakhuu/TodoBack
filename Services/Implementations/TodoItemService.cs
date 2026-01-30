using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;
using TodoBack.DTOs;
using TodoBack.DTOs.Auth;
using TodoBack.DTOs.TodoItem;
using TodoBack.Models;
using TodoBack.Repos.Interfaces;
using TodoBack.Services.Interfaces;

namespace TodoBack.Services.Implementations
{
    public class TodoItemService: ITodoItemService
    {
        private readonly ITodoItemRepository _repo;

        public TodoItemService(ITodoItemRepository repo)
        {
            _repo = repo;
        }

        public async Task<TodoItemResult> GetByIdAsync(int id, int userId)
        {
            var item = await _repo.GetByIdAsync(id);

            if (item == null || item.UserId != userId)
                return new TodoItemResult { Success = false, Error = "Item not found or deleted" };

            return new TodoItemResult
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                Success = true
            };
        }

        public async Task<IEnumerable<TodoItemResult>> GetByUserIdAsync(int userId)
        {
            var items = await _repo.GetByUserIdAsync(userId);

            if (items == null || !items.Any())
                return Enumerable.Empty<TodoItemResult>();

            return items.Select(item => new TodoItemResult
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                Success = true
            });
        }

        public async Task<TodoItemResult> CreateAsync(int userId, TodoCreateDto dto)
        {

            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.", nameof(dto.Title));

            var item = new TodoItem
            {
                UserId = userId,
                Title = dto.Title,
                CategoryId = dto.CategoryId,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(item);

            return new TodoItemResult
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                Success = true
            };
        }

        public async Task<TodoItemResult> UpdateAsync(int userId, long id, UpdateTodoDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var item = await _repo.GetByIdAsync(id);
            if (item == null || item.UserId != userId)
                return null;

            if (dto.Title != null)
                item.Title = dto.Title;

            if (dto.CategoryId.HasValue)
                item.CategoryId = dto.CategoryId.Value;

            await _repo.UpdateAsync(item);

            return new TodoItemResult
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                Success = true
            };
        }

        public async Task<TodoItemResult> DeleteAsync(int id, int userId)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || item.UserId != userId)
                return new TodoItemResult { Success = false, Error = "Item not found or deleted" };

            item.IsDeleted = true;
            await _repo.DeleteAsync(id);

            return new TodoItemResult 
            {
                Id= item.Id,
                Title= item.Title,
                CategoryId= item.CategoryId,
                Success = true
            };
        }

        public async Task<int> DeleteByUserIdAsync(int userId)
        {
            return await _repo.DeleteByUserIdAsync(userId);
        }

        public async Task<TodoItemResult> MarkAsCompletedAsync(int id, int userId)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || item.UserId != userId)
                return null;

            item.IsCompleted = true;
            await _repo.UpdateAsync(item);

            return new TodoItemResult
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId
            };
        }
    }
}