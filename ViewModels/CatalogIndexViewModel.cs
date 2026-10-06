using GameStore.Models;

namespace GameStore.ViewModels;

public class CatalogIndexViewModel
{
    public IReadOnlyCollection<StoreGame> Games { get; init; } = Array.Empty<StoreGame>();

    public IReadOnlyCollection<Category> Categories { get; init; } = Array.Empty<Category>();

    public IReadOnlyCollection<GamePlatform> Platforms { get; init; } = Array.Empty<GamePlatform>();

    public int? SelectedCategoryId { get; init; }

    public int? SelectedPlatformId { get; init; }

    public string? Search { get; init; }
}
