# POE Trade Helper

Path of Exile trade helper like [POE-TradeMacro](https://github.com/PoE-TradeMacro/POE-TradeMacro) but written in C# using [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia) and querying the official Path of Exile trade API (https://www.pathofexile.com/trade).

This program has been developed with cross-platform compatiblity in mind, but currently only supports Windows, because native implementations for Linux and MacOS for parts of the software are missing. Contributions are welcome.

# Features

Currently you can price check items by hovering over them ingame and pressing **CTRL + D**. An overlay with the query result shows up and stays open until you press **ESC**. You can move the overlay by dragging it with your mouse.

You can set filters to adjust the price query to your needs. Some filters are already preset under certain conditions (e.g. five or more links or Tier 1 stats), but can be modified. You can also open queries in your web browser.

![POETradeHelper](https://github.com/alueck/POE-TradeHelper/assets/9286842/03995496-8d81-4c07-88b9-b0823238414b)

If you press **ALT + W** while hovering over an item the related Wiki page is opened based on the Wiki configured in the settings.

This is still work in progress. So if you decide to try it out, you can be sure to run into at least some issues.

# License

This software is licensed under the MIT license (see [LICENSE](LICENSE)) and uses third party libraries that are licensed under their own terms and conditions . (see [LICENSE-3RD-PARTY.md](LICENSE-3RD-PARTY.md)).

