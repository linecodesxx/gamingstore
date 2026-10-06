using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public class SqliteGameCatalogRepository : IGameCatalogRepository
{
    private readonly GameStoreDbContext _dbContext;

    public SqliteGameCatalogRepository(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IReadOnlyCollection<Category> GetCategories()
    {
        return _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Title)
            .ToArray();
    }

    public IReadOnlyCollection<GamePlatform> GetPlatforms()
    {
        return _dbContext.GamePlatforms
            .AsNoTracking()
            .OrderBy(platform => platform.Title)
            .ToArray();
    }

    public IReadOnlyCollection<StoreGame> GetGames(int? categoryId = null, int? platformId = null, string? search = null)
    {
        var games = _dbContext.StoreGames
            .AsNoTracking()
            .Include(game => game.Category)
            .Include(game => game.SupportedPlatforms)
            .AsQueryable();

        if (categoryId is not null)
        {
            games = games.Where(game => game.CategoryId == categoryId);
        }

        if (platformId is not null)
        {
            games = games.Where(game => game.SupportedPlatforms.Any(platform => platform.PlatformId == platformId));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            games = games.Where(game =>
                EF.Functions.Like(game.Name, pattern) ||
                EF.Functions.Like(game.Studio, pattern) ||
                EF.Functions.Like(game.Summary, pattern));
        }

        return games
            .OrderBy(game => game.Name)
            .ToArray();
    }

    public StoreGame? GetGame(int gameId)
    {
        return _dbContext.StoreGames
            .AsNoTracking()
            .Include(game => game.Category)
            .Include(game => game.SupportedPlatforms)
            .FirstOrDefault(game => game.GameId == gameId);
    }
}
