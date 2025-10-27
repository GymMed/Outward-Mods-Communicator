## Changelog

### Release 1.1.0 Version

#### Added

-   Added support for optional parameter descriptions when registering events.
Example: <code>EventBus.RegisterEvent(
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
