using GameStore.Data;
using GameStore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

public class CatalogController : Controller
{
    private readonly IGameCatalogRepository _catalogRepository;

    public CatalogController(IGameCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public IActionResult Index(int? categoryId, int? platformId, string? search)
    {
        var viewModel = new CatalogIndexViewModel
        {
            Games = _catalogRepository.GetGames(categoryId, platformId, search),
            Categories = _catalogRepository.GetCategories(),
            Platforms = _catalogRepository.GetPlatforms(),
            SelectedCategoryId = categoryId,
            SelectedPlatformId = platformId,
            Search = search
        };

        return View(viewModel);
    }

    public IActionResult Details(int id)
    {
        var game = _catalogRepository.GetGame(id);
        if (game is null)
        {
            return NotFound();
        }

        return View(game);
    }
}
