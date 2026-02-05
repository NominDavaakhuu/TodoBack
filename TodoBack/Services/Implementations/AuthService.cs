using Shared.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TodoBack.DTOs.Auth;
using TodoBack.Models;
using TodoBack.Repos.Interfaces;
using TodoBack.Services.Interfaces;

namespace TodoBack.Services.Implementations
{
    public class AuthService: IAuthService
    {

        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;

        public AuthService(IUserRepository users, IPasswordHasher hasher)
        {
            _users = users;
            _hasher = hasher;
        }


        public async Task<AuthResult> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Username) || string.IsNullOrWhiteSpace(dto?.Password))
                return new AuthResult { Success = false, Error = "Username and password are required." };

            if (await _users.UserExistsAsync(dto.Username, dto.Email))
                return new AuthResult { Success = false, Error = "User already exists." };

            var user = new User
            {
                Username = dto.Username,
                UsernameNormalized = dto.Username.ToUpperInvariant(),
                Email = dto.Email,
                EmailNormalized = dto.Email?.ToUpperInvariant(),
                PasswordHash = _hasher.Hash(dto.Password)
            };

            var created = await _users.CreateAsync(user);

            return new AuthResult
            {
                Success = true,
                UserId = (int?)created.Id,
                Username = created.Username
            };
        }


        public async Task<AuthResult> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto?.Password))
                return new AuthResult { Success = false, Error = "Username and password are required." };

            var user = await _users.GetByUsernameAsync(dto.UsernameOrEmail) ?? await _users.GetByUserEmailAsync(dto.UsernameOrEmail);

            if (user == null || !_hasher.Verify(dto.Password, user.PasswordHash))
                return new AuthResult { Success = false, Error = "Invalid username or password." };

            return new AuthResult
            {
                Success = true,
                UserId = (int?)user.Id,
                Username = user.Username
            };
        }

    }
}