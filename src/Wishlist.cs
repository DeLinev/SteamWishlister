using System.Net;
using System.Net.Http.Json;

namespace SteamWishlister;

class Wishlist
{
    private readonly string baseAddress;
    private readonly string sessionId;
    private readonly string loginCookie;
    private readonly HttpClient httpClient;

    public Wishlist(string sessionId, string loginCookie)
    {
        baseAddress = "https://store.steampowered.com";
        this.sessionId = sessionId;
        this.loginCookie = loginCookie;

        CookieContainer coockieContainer = new();
        HttpClientHandler handler = new()
        {
            CookieContainer = coockieContainer
        };

        coockieContainer.Add(
            new Uri(baseAddress),
            new Cookie("sessionid", sessionId)
        );

        coockieContainer.Add(
            new Uri(baseAddress),
            new Cookie("steamLoginSecure", loginCookie)
        );

        httpClient = new(handler)
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    public async Task AddGameAsync(string gameId)
    {
        FormUrlEncodedContent content = new([
            new KeyValuePair<string, string>("sessionid", sessionId),
            new KeyValuePair<string, string>("appid", gameId)
        ]);

        var response = await httpClient.PostAsync("/api/addtowishlist", content);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<AddGameResponse>();
        if (body?.success ?? false)
            Console.WriteLine($"> Game {gameId} was successfully added to wishlist!");
        else
            Console.WriteLine($"! Failed to add game to wishlist.");
        Console.WriteLine($"> Games in wishlist: {body?.wishlistCount ?? 0}");
    }

    public async Task<string[]> GenerateNewQueue()
    {
        FormUrlEncodedContent content = new([
            new KeyValuePair<string, string>("sessionid", sessionId),
            new KeyValuePair<string, string>("queuetype", "0"), // no idea what queue type means
        ]);

        var response = await httpClient.PostAsync("/explore/generatenewdiscoveryqueue", content);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("Unauthorized. Pass valid --sessionid and --logincookie");
            return [];
        }

        var body = await response.Content.ReadFromJsonAsync<GenerateNewQueueResponse>();
        if (body == null)
            return [];

        string[] gameIds = Array.ConvertAll(body.queue, x => x.ToString());
        return gameIds;
    }
}

public record AddGameResponse(bool success, int wishlistCount);

public record GenerateNewQueueResponse(int[] queue);