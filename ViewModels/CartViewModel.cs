using GameStore.Models;

namespace GameStore.ViewModels;

public class CartViewModel
{
    public ShoppingCart Cart { get; init; } = new();

    public int TotalCount => Cart.Entries.Sum(entry => entry.Count);
}
