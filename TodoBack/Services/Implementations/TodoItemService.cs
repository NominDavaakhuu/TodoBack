using Shared.DTOs;
using Shared.DTOs.TodoItem;
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
using TodoBack.Models;
using TodoBack.Repos.Interfaces;
using TodoBack.Services.Interfaces;

namespace TodoBack.Services.Implementations
{


    public class TodoItemService : ITodoItemService
    {
        private readonly ITodoItemRepository _repo;

        public TodoItemService(ITodoItemRepository repo) => _repo = repo;

        public async Task<ApiResponse<TodoDto>> GetByIdAsync(long id, long userId)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || item.IsDeleted || item.UserId != userId)
                return ApiResponse<TodoDto>.Fail("Item not found or deleted");

            var dto = new TodoDto
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };

            return ApiResponse<TodoDto>.Ok(dto);
        }

        public async Task<ApiResponse<List<TodoDto>>> GetByUserIdAsync(long userId)
        {
            var items = await _repo.GetByUserIdAsync(userId);
            var list = items?
                .Where(i => !i.IsDeleted)
                .Select(i => new TodoDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    CategoryId = i.CategoryId,
                    IsCompleted = i.IsCompleted,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToList() ?? new List<TodoDto>();

            return ApiResponse<List<TodoDto>>.Ok(list);
        }

        public async Task<ApiResponse<TodoDto>> CreateAsync(long userId, TodoCreateDto dto)
        {
            if (dto == null) return ApiResponse<TodoDto>.Fail("Payload is required");
            if (string.IsNullOrWhiteSpace(dto.Title))
                return ApiResponse<TodoDto>.Fail("Title is required");

            var item = new TodoItem
            {
                UserId = userId,
                Title = dto.Title.Trim(),
                CategoryId = dto.CategoryId,
                IsCompleted = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _repo.AddAsync(item);

            var result = new TodoDto
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };

            return ApiResponse<TodoDto>.Ok(result);
        }

        public async Task<ApiResponse<TodoDto>> UpdateAsync(long userId, long id, UpdateTodoDto dto)
        {
            if (dto == null) return ApiResponse<TodoDto>.Fail("Payload is required");

            var item = await _repo.GetByIdAsync(id);
            if (item == null || item.IsDeleted || item.UserId != userId)
                return ApiResponse<TodoDto>.Fail("Item not found or deleted");

            if (dto.Title != null) item.Title = dto.Title.Trim();
            if (dto.CategoryId.HasValue) item.CategoryId = dto.CategoryId.Value;

            item.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(item);

            var result = new TodoDto
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };

            return ApiResponse<TodoDto>.Ok(result);
        }

        public async Task<ApiResponse<TodoDto>> DeleteAsync(long id, long userId)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || item.IsDeleted || item.UserId != userId)
                return ApiResponse<TodoDto>.Fail("Item not found or deleted");

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            await _repo.DeleteAsync(id);

            var result = new TodoDto
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };

            return ApiResponse<TodoDto>.Ok(result);
        }

        public async Task<ApiResponse<int>> DeleteByUserIdAsync(long userId)
        {
            var count = await _repo.DeleteByUserIdAsync(userId);
            return ApiResponse<int>.Ok(count);
        }

        public async Task<ApiResponse<TodoDto>> MarkAsCompletedAsync(long id, long userId)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || item.IsDeleted || item.UserId != userId)
                return ApiResponse<TodoDto>.Fail("Item not found or deleted");

            if (!item.IsCompleted)
            {
                item.IsCompleted = true;
                item.UpdatedAt = DateTime.UtcNow;
                await _repo.UpdateAsync(item);
            }

            var result = new TodoDto
            {
                Id = item.Id,
                Title = item.Title,
                CategoryId = item.CategoryId,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };

            return ApiResponse<TodoDto>.Ok(result);
        }
    }
}