using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public class GameStoreDbContext : DbContext
{
    public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<StoreGame> StoreGames => Set<StoreGame>();

    public DbSet<GamePlatform> GamePlatforms => Set<GamePlatform>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            entity.HasKey(category => category.CategoryId);
            entity.Property(category => category.Title).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<StoreGame>(entity =>
        {
            entity.ToTable("StoreGame");
            entity.HasKey(game => game.GameId);
            entity.Property(game => game.Name).HasMaxLength(140).IsRequired();
            entity.Property(game => game.Summary).HasMaxLength(600).IsRequired();
            entity.Property(game => game.Cost).HasPrecision(10, 2);
            entity.Property(game => game.Studio).HasMaxLength(120).IsRequired();
            entity.Property(game => game.ImageUrl).HasMaxLength(500).IsRequired();

            entity
                .HasOne(game => game.Category)
                .WithMany(category => category.GameList)
                .HasForeignKey(game => game.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GamePlatform>(entity =>
        {
            entity.ToTable("GamePlatform");
            entity.HasKey(platform => platform.PlatformId);
            entity.Property(platform => platform.Title).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<StoreGame>()
            .HasMany(game => game.SupportedPlatforms)
            .WithMany(platform => platform.AvailableGames)
            .UsingEntity<Dictionary<string, object>>(
                "GamePlatformLink",
                link => link
                    .HasOne<GamePlatform>()
                    .WithMany()
                    .HasForeignKey("PlatformId")
                    .OnDelete(DeleteBehavior.Cascade),
                link => link
                    .HasOne<StoreGame>()
                    .WithMany()
                    .HasForeignKey("GameId")
                    .OnDelete(DeleteBehavior.Cascade),
                link =>
                {
                    link.ToTable("GamePlatformLink");
                    link.HasKey("GameId", "PlatformId");
                });
    }
}
