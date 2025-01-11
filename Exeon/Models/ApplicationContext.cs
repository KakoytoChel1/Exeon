using Exeon.Models.Actions;
using Exeon.Models.Chat;
using Exeon.Models.Commands;
using Microsoft.EntityFrameworkCore;

namespace Exeon.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<MessageItem> MessageItems { get; set; } = null!;
        public DbSet<CustomCommand> CustomCommands { get; set; } = null!;
        public DbSet<Action> Actions { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Application.db");
        }
    }
}
