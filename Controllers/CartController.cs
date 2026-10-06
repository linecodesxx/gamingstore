using GameStore.Data;
using GameStore.Models;
using GameStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Controllers;

public class CartController : Controller
{
    private const string CartCookieName = "guest-cart-id";

    private readonly GameStoreDbContext _dbContext;
    private readonly IGameCatalogRepository _catalogRepository;

    public CartController(GameStoreDbContext dbContext, IGameCatalogRepository catalogRepository)
    {
        _dbContext = dbContext;
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

        var cart = GetOrCreateGuestCart();
        var entry = _dbContext.GuestCartEntries.FirstOrDefault(item =>
            item.GuestCartId == cart.GuestCartId && item.GameId == gameId);

        if (entry is null)
        {
            _dbContext.GuestCartEntries.Add(new GuestCartEntry
            {
                GuestCartId = cart.GuestCartId,
                GameId = gameId,
                Count = Math.Max(1, count)
            });
        }
        else
        {
            entry.Count += Math.Max(1, count);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();

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
        var cartId = GetCartIdFromCookie();
        if (cartId is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var entry = _dbContext.GuestCartEntries.FirstOrDefault(item =>
            item.GuestCartId == cartId.Value && item.GameId == gameId);

        if (entry is not null)
        {
            if (count < 1)
            {
                _dbContext.GuestCartEntries.Remove(entry);
            }
            else
            {
                entry.Count = count;
            }

            UpdateCartTimestamp(cartId.Value);
            _dbContext.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int gameId)
    {
        var cartId = GetCartIdFromCookie();
        if (cartId is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var entry = _dbContext.GuestCartEntries.FirstOrDefault(item =>
            item.GuestCartId == cartId.Value && item.GameId == gameId);

        if (entry is not null)
        {
            _dbContext.GuestCartEntries.Remove(entry);
            UpdateCartTimestamp(cartId.Value);
            _dbContext.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        var cartId = GetCartIdFromCookie();
        if (cartId is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var entries = _dbContext.GuestCartEntries.Where(item => item.GuestCartId == cartId.Value);
        _dbContext.GuestCartEntries.RemoveRange(entries);
        UpdateCartTimestamp(cartId.Value);
        _dbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    private ShoppingCart GetCart()
    {
        var cart = new ShoppingCart();
        var cartId = GetCartIdFromCookie();

        if (cartId is null)
        {
            return cart;
        }

        var entries = _dbContext.GuestCartEntries
            .AsNoTracking()
            .Include(entry => entry.Product)
            .ThenInclude(game => game.Category)
            .Where(entry => entry.GuestCartId == cartId.Value)
            .ToArray();

        foreach (var entry in entries)
        {
            cart.AddItem(entry.Product, entry.Count);
        }

        return cart;
    }

    private GuestCart GetOrCreateGuestCart()
    {
        var cartId = GetCartIdFromCookie();
        var cart = cartId is null
            ? null
            : _dbContext.GuestCarts.FirstOrDefault(item => item.GuestCartId == cartId.Value);

        if (cart is not null)
        {
            return cart;
        }

        cart = new GuestCart
        {
            GuestCartId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.GuestCarts.Add(cart);
        Response.Cookies.Append(CartCookieName, cart.GuestCartId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return cart;
    }

    private Guid? GetCartIdFromCookie()
    {
        return Request.Cookies.TryGetValue(CartCookieName, out var value) && Guid.TryParse(value, out var cartId)
            ? cartId
            : null;
    }

    private void UpdateCartTimestamp(Guid cartId)
    {
        var cart = _dbContext.GuestCarts.FirstOrDefault(item => item.GuestCartId == cartId);
        if (cart is not null)
        {
            cart.UpdatedAt = DateTime.UtcNow;
        }
    }
}
