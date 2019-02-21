using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{
    public class WeddingPlannerContext : DbContext
    {
        public WeddingPlannerContext(DbContextOptions<WeddingPlannerContext> options) : base(options) {}
        public DbSet<User> Users {get; set;}
        public DbSet<Logger> Loggers {get; set;}
        public DbSet<Wedding> Weddings {get; set;}
        public DbSet<Guest> Guests {get; set;}

        public void CreateUser(User newUser, HttpContext context)
        {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            Add(newUser);
            SaveChanges();
            User oneUser = Users.Last();
            int x = oneUser.UserId;
            context.Session.SetInt32("uid", x);
        }
    }
}