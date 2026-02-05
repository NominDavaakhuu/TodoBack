using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoBack.Models;

namespace TodoBack.Repos.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<bool> UserExistsAsync(string username, string email);
        Task<User> CreateAsync(User user);
        Task<User> GetByUserEmailAsync(string email);
    }
}
