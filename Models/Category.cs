namespace GameStore.Models;

public class Category
{
    public int CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public ICollection<StoreGame> GameList { get; set; } = new List<StoreGame>();
}
