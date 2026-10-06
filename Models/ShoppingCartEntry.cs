namespace GameStore.Models;

public class ShoppingCartEntry
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public StoreGame Product { get; set; } = new();

    public int Count { get; set; }

    public decimal CalculatePrice()
    {
        return Product.Cost * Count;
    }
}
