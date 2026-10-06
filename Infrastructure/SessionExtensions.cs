using System.Text.Json;

namespace GameStore.Infrastructure;

public static class SessionExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void SetObject<TValue>(this ISession session, string key, TValue value)
    {
        session.SetString(key, JsonSerializer.Serialize(value, JsonOptions));
    }

    public static TValue? GetObject<TValue>(this ISession session, string key)
    {
        var value = session.GetString(key);

        return value is null
            ? default
            : JsonSerializer.Deserialize<TValue>(value, JsonOptions);
    }
}
