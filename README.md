# Magnanibot
> A magnanimous, asynchronous, general-purpose Discord chatbot.

All responses are embedded. Makes heavy use of the [CommonBotLibrary](https://github.com/bcanseco/common-bot-library) for command sources. Supports pagination and other interactive messages via emoji reactions.

Built with [Discord.NET](http://github.com/rogueexception/discord.net). ❤

## Requirements
This is just my personal chatbot, so there isn't much documentation for it. If you'd like to host an instance, just follow these steps on Windows/Mac/Linux.

1. `git clone git@github.com:bcanseco/magnanibot.git`
2. `cd magnanibot`
2. Open the [tokens file](tokens.json) with your favorite editor and fill in your bot's [token](https://discordapp.com/developers/applications/me) for `Discord`.
   * Commands that pull from third-party APIs require keys; click [here](https://github.com/bcanseco/common-bot-library#services) for more info.
   * The [Memory](src/Magnanibot.Discord/Modules/Memory.cs) and [Trophy](src/Magnanibot.Discord/Modules/Trophy.cs) commands pull from the database denoted by the `MySql` property. Check the [Context folder](src/Magnanibot.Discord/Context) for schema details.
   * You can optionally also fill in the `Alias` value to use a name other than "Magnanibot".
3. `cd src/Magnanibot.Discord`
4. `dotnet restore`
5. `dotnet run`

## Contributing
Pull requests, issues, or any questions/comments are welcome. I'll usually reply within a day.
