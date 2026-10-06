using GameStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.ViewComponents;

public class CartSummaryViewComponent : ViewComponent
{
    private const string CartCookieName = "guest-cart-id";

    private readonly GameStoreDbContext _dbContext;

    public CartSummaryViewComponent(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IViewComponentResult Invoke()
    {
        if (!Request.Cookies.TryGetValue(CartCookieName, out var value) || !Guid.TryParse(value, out var cartId))
        {
            return View(0);
        }

        var count = _dbContext.GuestCartEntries
            .Where(entry => entry.GuestCartId == cartId)
            .Sum(entry => (int?)entry.Count) ?? 0;

        return View(count);
    }
}
