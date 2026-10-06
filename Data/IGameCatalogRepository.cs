using GameStore.Models;

namespace GameStore.Data;

public interface IGameCatalogRepository
{
    IReadOnlyCollection<Category> GetCategories();

    IReadOnlyCollection<GamePlatform> GetPlatforms();

    IReadOnlyCollection<StoreGame> GetGames(int? categoryId = null, int? platformId = null, string? search = null);

    StoreGame? GetGame(int gameId);
}
