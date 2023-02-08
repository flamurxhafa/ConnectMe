using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Models;
using ProfileMatching.Models.ViewModels;

namespace ProfileMatching.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users {  get; set; }
        public DbSet<Interactions> Interactions {  get; set; }
        public DbSet<Match> Matches {  get; set; }
    }
}
