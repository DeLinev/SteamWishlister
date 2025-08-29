# SteamWishlister

Ever dreamed of adding entire Steam Store to your wishlist?

## How to Use
```
Description:
  App for managing Steam Wishlist

Usage:
  SteamWishlister [command] [options]

Options:
  -?, -h, --help                Show help and usage information
  --version                     Show version information
  -s, --sessionid (REQUIRED)    Steam session id.
  -l, --logincookie (REQUIRED)  Steam login cookie.

Commands:
  wishlist  Manage Steam Wishlist.
```

## Wishlist Commands
```
Commands:
  add       Add a game to wishlist
  genqueue  Generate new queue of games
  autoadd   Automatically add games to your wishlist
```

To get help, type the needed command with the `--help` option.

To use any available command, you need to provide 2 required options: `--sessionid` and `--logincookie`. These are cookies that allow the program to make requests from your Steam account (as if you were doing this yourself). You can find them in your browser by going to the Steam page and logging into your account, then:
- Open Developer Tools (press F12)
- Go to the Application tab
- On the left navbar, choose "Cookies"
- Search for `sessionid` and `steamLoginSecure`
- Provide these values for `--sessionid` and `--logincookie` respectively

## How to Add All Games to Your Wishlist
```
SteamWishlister wishlist autoadd --sessionid <your session id> --logincookie <your login cookie>
```