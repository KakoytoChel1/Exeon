using Exeon.Models.Actions;
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

        public DbSet<CustomCommand> CustomCommands { get; set; } = null!;
        public DbSet<Action> Actions { get; set; } = null!;
        public DbSet<FileAction> FileActions { get; set; } = null!;
        public DbSet<WebAction> WebActions { get; set; } = null!;
        public DbSet<PauseAction> PauseActions { get; set; } = null!;
        public DbSet<SystemBrightnessAction> BrightnessActions { get; set; } = null!;
        public DbSet<SystemSoundAction> SoundActions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\KakoytoChel228\\Desktop\\Application.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomCommand>()
                .HasMany(c => c.Actions)
                .WithOne(a => a.RootCommand)
                .HasForeignKey(a => a.RootCommandId);

            modelBuilder.Entity<FileAction>().HasBaseType<Action>();
            modelBuilder.Entity<WebAction>().HasBaseType<Action>();
            modelBuilder.Entity<PauseAction>().HasBaseType<Action>();
            modelBuilder.Entity<SystemBrightnessAction>().HasBaseType<Action>();
            modelBuilder.Entity<SystemSoundAction>().HasBaseType<Action>();

            base.OnModelCreating(modelBuilder);
        }
    }

}
