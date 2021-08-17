namespace GameManager.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GameTrackerContext : DbContext
    {
        public GameTrackerContext()
            : base("name=GameTrackerContext")
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GameSystem> GameSystems { get; set; }
        public virtual DbSet<GameUser> GameUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSystem>()
                .HasMany(e => e.Games)
                .WithRequired(e => e.GameSystem)
                .HasForeignKey(e => e.SystemName)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<GameUser>()
                .HasMany(e => e.Games)
                .WithRequired(e => e.GameUser)
                .WillCascadeOnDelete(false);
        }
    }
}
