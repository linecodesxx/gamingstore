namespace GameStore.Models;

public class GuestCart
{
    public Guid GuestCartId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<GuestCartEntry> Entries { get; set; } = new List<GuestCartEntry>();
}
