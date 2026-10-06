using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GameStore.ViewComponents;

public class CartSummaryViewComponent : ViewComponent
{
    private const string CartSessionKey = "guest-cart";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public IViewComponentResult Invoke()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        var lines = string.IsNullOrWhiteSpace(json)
            ? new List<CartSessionLine>()
            : JsonSerializer.Deserialize<List<CartSessionLine>>(json, JsonOptions) ?? new List<CartSessionLine>();

        var count = lines.Sum(line => line.Count);

        return View(count);
    }

    private sealed record CartSessionLine(int GameId, int Count);
}
