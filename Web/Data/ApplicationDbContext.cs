using FNS.Main.Models;

namespace FNS.Main.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<News> News { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<AdminCall> Calls { get; set; }
        public DbSet<Voting> Votings { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<VotedUsers> VotedUsers { get; set; }
    }
}
