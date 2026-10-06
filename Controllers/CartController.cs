using GameStore.Data;
using GameStore.Infrastructure;
using GameStore.Models;
using GameStore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

public class CartController : Controller
{
    private const string CartSessionKey = "guest-cart";

    private readonly IGameCatalogRepository _catalogRepository;

    public CartController(IGameCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public IActionResult Index()
    {
        return View(new CartViewModel { Cart = GetCart() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int gameId, int count = 1, string? returnUrl = null)
    {
        var game = _catalogRepository.GetGame(gameId);
        if (game is null)
        {
            return NotFound();
        }

        var cart = GetCart();
        cart.AddItem(game, count);
        SaveCart(cart);

        TempData["CartMessage"] = $"{game.Name} добавлена в корзину.";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int gameId, int count)
    {
        var cart = GetCart();
        cart.SetItemCount(gameId, count);
        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int gameId)
    {
        var cart = GetCart();
        cart.DeleteItem(gameId);
        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        var cart = GetCart();
        cart.RemoveAll();
        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }

    private ShoppingCart GetCart()
    {
        var lines = HttpContext.Session.GetObject<List<CartSessionLine>>(CartSessionKey) ?? new List<CartSessionLine>();
        var cart = new ShoppingCart();

        foreach (var line in lines)
        {
            var game = _catalogRepository.GetGame(line.GameId);
            if (game is not null)
            {
                cart.AddItem(game, line.Count);
            }
        }

        return cart;
    }

    private void SaveCart(ShoppingCart cart)
    {
        var lines = cart.Entries.Select(entry => new CartSessionLine(entry.GameId, entry.Count)).ToList();
        HttpContext.Session.SetObject(CartSessionKey, lines);
    }

    private sealed record CartSessionLine(int GameId, int Count);
}
