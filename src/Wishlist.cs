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
        int maxRetries = 3;
        int delayMilliseconds = 20000;

        for (int i = 0; i < maxRetries; i++)
        {
            FormUrlEncodedContent content = new([
                new KeyValuePair<string, string>("sessionid", sessionId),
                new KeyValuePair<string, string>("appid", gameId)
            ]);

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/addtowishlist");
            request.Headers.Referrer = new Uri($"https://store.steampowered.com/app/{gameId}");
            request.Content = content;

            if (i > 0) Console.WriteLine($"Retrying add to wishlist... (Attempt {i + 1}/{maxRetries})");

            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<AddGameResponse>();
                if (body?.success ?? false)
                {
                    Console.WriteLine($"> Game {gameId} was successfully added to wishlist!");
                    Console.WriteLine($"> Games in wishlist: {body?.wishlistCount ?? 0}");
                    return;
                }
                else
                {
                    Console.WriteLine($"! API returned success: false for game {gameId}.");
                }
            }

            Console.WriteLine($"! HTTP Error: {response.StatusCode}. Backing off for {delayMilliseconds / 1000} seconds.");
            await Task.Delay(delayMilliseconds);
        }

        Console.WriteLine($"! Failed to add game {gameId} after {maxRetries} attempts.");
    }

    public async Task<string[]> GenerateNewQueue()
    {
        int maxRetries = 3;
        int delayMilliseconds = 25000;

        for (int i = 0; i < maxRetries; i++)
        {
            FormUrlEncodedContent content = new([
                new KeyValuePair<string, string>("sessionid", sessionId),
                new KeyValuePair<string, string>("queuetype", "0"),
            ]);

            if (i > 0) Console.WriteLine($"[Anti-Bot] Retrying queue generation... (Attempt {i + 1}/{maxRetries})");

            var response = await httpClient.PostAsync("/explore/generatenewdiscoveryqueue", content);

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadFromJsonAsync<GenerateNewQueueResponse>();
                if (body == null || body.queue == null) return [];

                string[] gameIds = Array.ConvertAll(body.queue, x => x.ToString());
                Console.WriteLine($"-> Successfully generated queue with {gameIds.Length} games.");
                return gameIds;
            }

            Console.WriteLine($"! HTTP Error: {response.StatusCode}. Backing off for {delayMilliseconds / 1000} seconds.");
            await Task.Delay(delayMilliseconds);
        }

        Console.WriteLine($"! Failed to generate queue after {maxRetries} attempts. Pass valid --sessionid and --logincookie.");
        return [];
    }
}

public record AddGameResponse(bool success, int wishlistCount);

public record GenerateNewQueueResponse(int[] queue);