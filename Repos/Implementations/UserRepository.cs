using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TodoBack.Models;
using System.Data.Entity;
using TodoBack.Data;
using TodoBack.Repos.Interfaces;

namespace TodoBack.Repos.Implementations
{
    public class UserRepository: IUserRepository
    {

        private readonly TodoDbContext _db;
        public UserRepository(TodoDbContext db) => _db = db;

        public Task<User> GetByUsernameAsync(string username)
            => _db.Users.SingleOrDefaultAsync(u => u.Username == username);

        public Task<bool> UsernameExistsAsync(string username)
            => _db.Users.AnyAsync(u => u.Username == username);

        public async Task<User> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}