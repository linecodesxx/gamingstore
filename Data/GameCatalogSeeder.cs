using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public static class GameCatalogSeeder
{
    public static void Seed(GameStoreDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
        EnsureCartTables(dbContext);
        RemoveBrokenRows(dbContext);

        if (dbContext.StoreGames.Any())
        {
            return;
        }

        var action = new Category { Title = "Экшен" };
        var adventure = new Category { Title = "Приключения" };
        var racing = new Category { Title = "Гонки" };
        var rpg = new Category { Title = "RPG" };
        var sports = new Category { Title = "Спорт" };

        var playStation = new GamePlatform { Title = "PlayStation 5" };
        var xbox = new GamePlatform { Title = "Xbox Series X|S" };
        var nintendo = new GamePlatform { Title = "Nintendo Switch" };

        var games = new[]
        {
            CreateGame(
                "Spider-Man 2",
                "Кинематографичный экшен про двух Человеков-пауков, быстрые полеты над Нью-Йорком и битвы с Веномом.",
                4290,
                new DateTime(2023, 10, 20),
                "Insomniac Games",
                "https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=900&q=80",
                action,
                playStation),
            CreateGame(
                "Forza Horizon 5",
                "Открытый фестиваль скорости в Мексике: сотни машин, живые сезоны и свободные заезды с друзьями.",
                3490,
                new DateTime(2021, 11, 9),
                "Playground Games",
                "https://images.unsplash.com/photo-1503736334956-4c8f8e92946d?auto=format&fit=crop&w=900&q=80",
                racing,
                xbox),
            CreateGame(
                "The Legend of Zelda: Tears of the Kingdom",
                "Большое приключение в Хайруле с небесными островами, конструктором механизмов и свободой решений.",
                5390,
                new DateTime(2023, 5, 12),
                "Nintendo",
                "https://images.unsplash.com/photo-1578303512597-81e6cc155b3e?auto=format&fit=crop&w=900&q=80",
                adventure,
                nintendo),
            CreateGame(
                "EA Sports FC 26",
                "Футбол для быстрых матчей на диване и сетевых турниров: клубы, карьера и режим Ultimate Team.",
                3990,
                new DateTime(2026, 9, 25),
                "EA Sports",
                "https://images.unsplash.com/photo-1579952363873-27f3bade9f55?auto=format&fit=crop&w=900&q=80",
                sports,
                playStation,
                xbox,
                nintendo),
            CreateGame(
                "Cyberpunk 2077: Ultimate Edition",
                "Неоновая RPG про наемника в Найт-Сити, где импланты, выборы и стиль решают почти все.",
                4590,
                new DateTime(2023, 12, 5),
                "CD Projekt Red",
                "https://images.unsplash.com/photo-1511512578047-dfb367046420?auto=format&fit=crop&w=900&q=80",
                rpg,
                playStation,
                xbox),
            CreateGame(
                "Super Mario Odyssey",
                "Яркое платформенное приключение с путешествием по королевствам и десятками находок на каждом уровне.",
                4890,
                new DateTime(2017, 10, 27),
                "Nintendo",
                "https://images.unsplash.com/photo-1612404730960-5c71577fca11?auto=format&fit=crop&w=900&q=80",
                adventure,
                nintendo),
            CreateGame(
                "Halo Infinite",
                "Научно-фантастический шутер про Мастера Чифа, кольцо Zeta Halo и масштабные арены.",
                2990,
                new DateTime(2021, 12, 8),
                "343 Industries",
                "https://images.unsplash.com/photo-1550745165-9bc0b252726f?auto=format&fit=crop&w=900&q=80",
                action,
                xbox),
            CreateGame(
                "Hogwarts Legacy",
                "Фэнтези-RPG в школе магии: занятия, дуэли, исследования замка и собственный стиль волшебника.",
                3790,
                new DateTime(2023, 2, 10),
                "Avalanche Software",
                "https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=900&q=80",
                rpg,
                playStation,
                xbox,
                nintendo)
        };

        dbContext.StoreGames.AddRange(games);
        dbContext.SaveChanges();
    }

    private static void EnsureCartTables(GameStoreDbContext dbContext)
    {
        dbContext.Database.ExecuteSqlRaw(
            """
            CREATE TABLE IF NOT EXISTS "GuestCart" (
                "GuestCartId" TEXT NOT NULL CONSTRAINT "PK_GuestCart" PRIMARY KEY,
                "CreatedAt" TEXT NOT NULL,
                "UpdatedAt" TEXT NOT NULL
            );
            """);

        dbContext.Database.ExecuteSqlRaw(
            """
            CREATE TABLE IF NOT EXISTS "GuestCartEntry" (
                "GuestCartEntryId" INTEGER NOT NULL CONSTRAINT "PK_GuestCartEntry" PRIMARY KEY AUTOINCREMENT,
                "GuestCartId" TEXT NOT NULL,
                "GameId" INTEGER NOT NULL,
                "Count" INTEGER NOT NULL,
                CONSTRAINT "FK_GuestCartEntry_GuestCart_GuestCartId" FOREIGN KEY ("GuestCartId") REFERENCES "GuestCart" ("GuestCartId") ON DELETE CASCADE,
                CONSTRAINT "FK_GuestCartEntry_StoreGame_GameId" FOREIGN KEY ("GameId") REFERENCES "StoreGame" ("GameId") ON DELETE CASCADE
            );
            """);

        dbContext.Database.ExecuteSqlRaw(
            """
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_GuestCartEntry_GuestCartId_GameId"
            ON "GuestCartEntry" ("GuestCartId", "GameId");
            """);

        dbContext.Database.ExecuteSqlRaw(
            """
            CREATE INDEX IF NOT EXISTS "IX_GuestCartEntry_GameId"
            ON "GuestCartEntry" ("GameId");
            """);
    }

    private static void RemoveBrokenRows(GameStoreDbContext dbContext)
    {
        dbContext.Database.ExecuteSqlRaw("""DELETE FROM "GuestCartEntry" WHERE "GameId" IN (SELECT "GameId" FROM "StoreGame" WHERE "Name" = '');""");
        dbContext.Database.ExecuteSqlRaw("""DELETE FROM "StoreGame" WHERE "Name" = '';""");
        dbContext.Database.ExecuteSqlRaw("""DELETE FROM "Category" WHERE "Title" = '';""");
    }

    private static StoreGame CreateGame(
        string name,
        string summary,
        decimal cost,
        DateTime publishedAt,
        string studio,
        string imageUrl,
        Category category,
        params GamePlatform[] platforms)
    {
        return new StoreGame
        {
            Name = name,
            Summary = summary,
            Cost = cost,
            PublishedAt = publishedAt,
            Studio = studio,
            ImageUrl = imageUrl,
            Category = category,
            SupportedPlatforms = platforms
        };
    }
}
