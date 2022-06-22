using Microsoft.EntityFrameworkCore;

using core_graph_v2.Models;

using HotChocolate.Resolvers;
using HotChocolate.Language;

namespace core_graph_v2.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Token> Token { get; set; }

        public DbSet<Action> Action { get; set; }
        public DbSet<ActionCmt> ActionCmt { get; set; }

        public DbSet<ClashCheck> ClashCheck { get; set; }
    }
}