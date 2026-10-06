namespace GameStore.Models;

public class GamePlatform
{
    public int PlatformId { get; set; }

    public string Title { get; set; } = string.Empty;

    public ICollection<StoreGame> AvailableGames { get; set; } = new List<StoreGame>();
}
