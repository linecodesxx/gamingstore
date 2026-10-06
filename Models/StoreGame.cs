namespace GameStore.Models;

public class StoreGame
{
    public int GameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public DateTime PublishedAt { get; set; }

    public string Studio { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = new();

    public ICollection<GamePlatform> SupportedPlatforms { get; set; } = new List<GamePlatform>();
}
