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

        addCommand.SetAction(async parseresult =>
        {
            string? sessionId = parseresult.GetValue(sessionIdOption);
            string? loginCookie = parseresult.GetValue(loginCookieOption);
            string? gameId = parseresult.GetValue(gameIdOption);

            Wishlist wishlist = new(sessionId, loginCookie);
            await wishlist.AddGameAsync(gameId);
        });

        await rootCommand.Parse(args).InvokeAsync();
    }
}