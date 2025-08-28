using System.CommandLine;

namespace SteamWishlister;

class Program
{
    static async Task Main(string[] args)
    {
        Option<string> sessionIdOption = new("--sessionid")
        {
            Description = "Steam session id.",
            Required = true,
            Recursive = true,
            Aliases = { "-s" }
        };
        Option<string> loginCookieOption = new("--logincookie")
        {
            Description = "Steam login cookie.",
            Required = true,
            Recursive = true,
            Aliases = { "-l" }
        };
        Option<string> gameIdOption = new("--gameid")
        {
            Description = "Steam game id.",
            Required = true,
            Aliases = { "-gid" }
        };

        RootCommand rootCommand = new("App for managing Steam Wishlist");
        rootCommand.Options.Add(sessionIdOption);
        rootCommand.Options.Add(loginCookieOption);

        Command wishlistCommand = new("wishlist", "Work with Steam Wishlist.");
        rootCommand.Subcommands.Add(wishlistCommand);

        Command addCommand = new("add", "Add a game to wishlist")
        {
            gameIdOption
        };
        wishlistCommand.Subcommands.Add(addCommand);

        Command generateQueueCommand = new("genqueue", "Generate new queue of games");
        wishlistCommand.Subcommands.Add(generateQueueCommand);

        Command autoAddCommand = new("autoadd", "Automatically adds games to your wishlist.");
        wishlistCommand.Subcommands.Add(autoAddCommand);

        addCommand.SetAction(async parseresult =>
        {
            string? sessionId = parseresult.GetValue(sessionIdOption) ?? "";
            string? loginCookie = parseresult.GetValue(loginCookieOption) ?? "";
            string? gameId = parseresult.GetValue(gameIdOption) ?? "";

            Wishlist wishlist = new(sessionId, loginCookie);
            await wishlist.AddGameAsync(gameId);
        });

        generateQueueCommand.SetAction(async parseResult =>
        {
            Console.WriteLine("Generating new game queue...");
            string sessionId = parseResult.GetValue(sessionIdOption) ?? "";
            string loginCookie = parseResult.GetValue(loginCookieOption) ?? "";

            Wishlist wishlist = new(sessionId, loginCookie);
            string[] gameIds = await wishlist.GenerateNewQueue();
            if (!gameIds.Any())
                return;

            Console.WriteLine("Game ids:");
            foreach (string gameId in gameIds)
            {
                Console.WriteLine(gameId);
            }
        });

        autoAddCommand.SetAction(async parseResult =>
        {
            Console.WriteLine("Starting the process of automatically adding games to the wishlist.");
            string sessionId = parseResult.GetValue(sessionIdOption) ?? "";
            string loginCookie = parseResult.GetValue(loginCookieOption) ?? "";

            Wishlist wishlist = new(sessionId, loginCookie);

            while (true)
            {
                try
                {
                    var gameIds = await wishlist.GenerateNewQueue();
                    if (!gameIds.Any())
                    {
                        Console.WriteLine("Game queue wan't retrieved.");
                        return;
                    }

                    Console.WriteLine("Game queue retrieved.");
                    await Task.WhenAll(gameIds.Select(wishlist.AddGameAsync));
                    Thread.Sleep(5000);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occurred: {e.Message}");
                }
            }
        });

        await rootCommand.Parse(args).InvokeAsync();
    }
}