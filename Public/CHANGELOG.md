## Changelog

### Release 1.2.0 Version

#### Added

-   Added support for optional event description when registering events.
Example: 
<br><code>EventBus.RegisterEvent(
    GUID,
    "yourEvent",
    "your event description",
    ("yourVariableRetrievalKey", typeof(string), "Your variable description.")
);</code>

#### Fixed

-   Fixed readme code blocks.
-   Changed how `EventBusDataPresenter` shows logs. Now it is much clearer.

### Release 1.1.0 Version

#### Added

-   Added support for optional parameter descriptions when registering events.
Example: 
<br><code>EventBus.RegisterEvent(
    GUID,
    "EnchantmentMenu@TryEnchant",
    ("menu", typeof(EnchantmentMenu), "The enchantment menu instance that invoked the TryEnchant method.")
);</code>
-   Added more text to readme description

#### Fixed

-   Moved the `PlayerModsOverrides.example.xml` file to
    `BepInEx/config/gymmed.Mods_Communicator/` to ensure configuration files
    remain untouched after updates — even after renaming `.example` to enable
    custom overrides.

### Release 1.0.1 Version

#### Fixed

-   Made sure to load `PlayerModsOverrides.xml` even if `xmlFilePath` variable
    is not provided from mods
-   Changed readme description

### Release 1.0.0 Version

-   Initial release.
