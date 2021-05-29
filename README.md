# Deep Rock Galactic Modded Save Helper
This is a small tool that will automatically manage your Deep Rock Galactic savegames for you, ensuring that your "modded" and "unmodded" savegames are constantly in sync with each other.

# Installation
Grab the latest version from the Releases page, and unpack the .exe into your Deep Rock Galactic folder. On default Steam installations, this is in `C:\Program Files (x86)\Steam\steamapps\common\Deep Rock Galactic`.
You can also go to the game's properties in Steam, and click `Browse local files`.

# Usage
Simply run the `DRGModdedSaveHelper.exe` file to start up the game every time. Do not directly start Deep Rock Galactic, this tool will do it for you.
When Deep Rock Galactic closes, it will automatically copy your savegames in the way that you specified in the config (by default, it copies the newer save to your older one).

# Configuration
After running the tool once, a config JSON file will be generated named `DRGModdedSaveHelper.json`. You can open this file to change some of the settings.

`CopyStrategy`: Determines how to copy your save files. Default is `newest`, which will take your vanilla and modded saves, and copy the newer save over the older one.
You can also use `alwaysModded` to always overwrite your vanilla save with your modded save, or `alwaysVanilla` to always overwrite your modded save with your vanilla save instead.

`KeepConsoleWindowOpen`: Whether the save helper console window should stay open after closing the game. Can be useful for debugging. Defaults to `false`.

`VerboseLogging`: Logs more info to the console screen, such as listing all found savegames. Can be useful for debugging.
