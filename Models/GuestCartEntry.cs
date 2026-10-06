namespace GameStore.Models;

public class GuestCartEntry
{
    public int GuestCartEntryId { get; set; }

    public Guid GuestCartId { get; set; }

    public GuestCart Cart { get; set; } = null!;

    public int GameId { get; set; }

    public StoreGame Product { get; set; } = null!;

    public int Count { get; set; }
}
