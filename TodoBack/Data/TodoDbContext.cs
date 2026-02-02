using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using TodoBack.Models;

namespace TodoBack.Data
{
    public class TodoDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public TodoDbContext() : base("name=TodoDbContext")
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- TodoItem -> User (many todos to one user) ----
            modelBuilder.Entity<TodoItem>()
                .HasRequired(t => t.User)
                .WithMany(u=> u.TodoItems)
                .HasForeignKey(t => t.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TodoItem>()
                .HasOptional(t => t.Category)
                .WithMany(c=> c.TodoItems)
                .HasForeignKey(t => t.CategoryId)
                .WillCascadeOnDelete(false);
        }
    }
}
