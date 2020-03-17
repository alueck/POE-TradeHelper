# POE Trade Helper

Path of Exile trade helper like [POE-TradeMacro](https://github.com/PoE-TradeMacro/POE-TradeMacro) but written in C# using [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia) and querying the official Path of Exile trade API (https://www.pathofexile.com/trade).

This program has been developed with cross-platform compatiblity in mind, but currently only supports Windows, because native implementations for Linux and MacOS for parts of the software are missing. Contributions are welcome.

# Functionality

Currently you can price check items by hovering over them ingame and pressing **CTRL + D**. An overlay with the query result shows up and stays open until you press **ESC**. You can move the overlay by dragging it with your mouse.

At the moment some filters are preset under certain conditions (e.g. five or more links), but you can not yet see or edit them via the user interface. But you can already select modifiers and adjust their values for a more advanced query. You can also open a query in your web browser.

This is still work in progress. So if you decide to try it out, you can be sure to run into at least some issues.

# License

This software is licensed under the MIT license (see [LICENSE.md](LICENSE.md)) and uses third party libraries that are licensed under their own terms and conditions . (see [LICENSE-3RD-PARTY.md](LICENSE-3RD-PARTY.md)).

