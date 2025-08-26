using System.Net;

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
        Console.WriteLine($"Adding game {gameId} to wishlist...");
        FormUrlEncodedContent content = new([
            new KeyValuePair<string, string>("sessionid", sessionId),
            new KeyValuePair<string, string>("appid", gameId)
        ]);

        var response = await httpClient.PostAsync("/api/addtowishlist", content);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {body}");
        Console.WriteLine("Game was successfully added!");
    }
}