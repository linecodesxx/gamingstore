namespace GameStore.Models;

public class ShoppingCart
{
    public ICollection<ShoppingCartEntry> Entries { get; set; } = new List<ShoppingCartEntry>();

    public decimal GetTotal()
    {
        return Entries.Sum(entry => entry.CalculatePrice());
    }

    public void AddItem(StoreGame game, int count = 1)
    {
        if (count < 1)
        {
            count = 1;
        }

        var currentEntry = Entries.FirstOrDefault(entry => entry.GameId == game.GameId);
        if (currentEntry is null)
        {
            Entries.Add(new ShoppingCartEntry
            {
                Id = Entries.Count == 0 ? 1 : Entries.Max(entry => entry.Id) + 1,
                GameId = game.GameId,
                Product = game,
                Count = count
            });

            return;
        }

        currentEntry.Count += count;
    }

    public void DeleteItem(int gameId)
    {
        var currentEntry = Entries.FirstOrDefault(entry => entry.GameId == gameId);
        if (currentEntry is not null)
        {
            Entries.Remove(currentEntry);
        }
    }

    public void SetItemCount(int gameId, int count)
    {
        if (count < 1)
        {
            DeleteItem(gameId);
            return;
        }

        var currentEntry = Entries.FirstOrDefault(entry => entry.GameId == gameId);
        if (currentEntry is not null)
        {
            currentEntry.Count = count;
        }
    }

    public void RemoveAll()
    {
        Entries.Clear();
    }
}
