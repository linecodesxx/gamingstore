using GameStore.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.ViewComponents;

public class CartSummaryViewComponent : ViewComponent
{
    private const string CartSessionKey = "guest-cart";

    public IViewComponentResult Invoke()
    {
        var lines = HttpContext.Session.GetObject<List<CartSessionLine>>(CartSessionKey) ?? new List<CartSessionLine>();
        var count = lines.Sum(line => line.Count);

        return View(count);
    }

    private sealed record CartSessionLine(int GameId, int Count);
}
