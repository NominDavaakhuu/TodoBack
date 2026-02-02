namespace TodoBack.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TodoBack.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TodoBack.Data.TodoDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TodoBack.Data.TodoDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            // ----- TEST USER SEED -----
            const string testUsername = "testUser";
            const string testEmail = "testuser@example.com";
            const string testPassword = "Test@12345"; 

            var usernameNorm = Normalize(testUsername);
            var emailNorm = Normalize(testEmail);

            // Does user already exist?
            var existing = context.Users.FirstOrDefault(u => u.UsernameNormalized == usernameNorm || u.EmailNormalized == emailNorm);

            if (existing == null)
            {
                var user = new User
                {
                    Username = testUsername,
                    UsernameNormalized = usernameNorm,
                    Email = testEmail,
                    EmailNormalized = emailNorm,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(testPassword, workFactor: 12),
                    CreatedAtUtc = DateTime.UtcNow
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
            var seededUser = context.Users.First(u => u.UsernameNormalized == usernameNorm);
        }
        private static string Normalize(string input)=> (input ?? string.Empty).Trim().ToUpperInvariant();
    }
}
