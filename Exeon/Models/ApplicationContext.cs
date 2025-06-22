using Exeon.Models.Actions;
using Exeon.Models.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Action = Exeon.Models.Actions.Action;

namespace Exeon.Models
{
    public class ApplicationContext : DbContext
    {
        private static string? _databasePath;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public static async Task InitializeDatabasePathAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile dbFile = await localFolder.CreateFileAsync("Application.db", CreationCollisionOption.OpenIfExists);
            _databasePath = dbFile.Path;
        }

        public DbSet<CustomCommand> CustomCommands { get; set; } = null!;
        public DbSet<TriggerCommand> TriggerCommands { get; set; } = null!;
        public DbSet<Action> Actions { get; set; } = null!;
        public DbSet<FileAction> FileActions { get; set; } = null!;
        public DbSet<WebAction> WebActions { get; set; } = null!;
        public DbSet<PauseAction> PauseActions { get; set; } = null!;
        public DbSet<SystemBrightnessAction> BrightnessActions { get; set; } = null!;
        public DbSet<SystemSoundAction> SoundActions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_databasePath == null)
                throw new InvalidOperationException("Database path not initialized. Call InitializeDatabasePathAsync first.");

            optionsBuilder.UseSqlite($"Data Source={_databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomCommand>()
                .HasMany(c => c.Actions)
                .WithOne(a => a.RootCommand)
                .HasForeignKey(a => a.RootCommandId);

            modelBuilder.Entity<CustomCommand>()
                .HasMany(c => c.TriggerCommands)
                .WithOne(tc => tc.RootCommand)
                .HasForeignKey(tc => tc.RootCommandId);

            modelBuilder.Entity<FileAction>().HasBaseType<Action>();
            modelBuilder.Entity<WebAction>().HasBaseType<Action>();
            modelBuilder.Entity<PauseAction>().HasBaseType<Action>();
            modelBuilder.Entity<SystemBrightnessAction>().HasBaseType<Action>();
            modelBuilder.Entity<SystemSoundAction>().HasBaseType<Action>();

            base.OnModelCreating(modelBuilder);
        }
    }

}
