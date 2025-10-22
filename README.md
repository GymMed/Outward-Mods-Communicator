<h1 align="center">
    Outward Mods Communicator
</h1>
<br/>
<div align="center">
  <img src="https://raw.githubusercontent.com/GymMed/Outward-Mods-Communicator/refs/heads/main/preview/images/Logo.png" alt="Outward game setting to require enchantment recipe when enchanting."/>
</div>

<div align="center">
	<a href="https://thunderstore.io/c/outward/p/GymMed/ModsCommunicator/">
		<img src="https://img.shields.io/thunderstore/dt/GymMed/ModsCommunicator" alt="Thunderstore Downloads">
	</a>
	<a href="https://github.com/GymMed/Outward-Mods-Communicator/releases/latest">
		<img src="https://img.shields.io/thunderstore/v/GymMed/ModsCommunicator" alt="Thunderstore Version">
	</a>
</div>

Outward Mods Communicator enables seamless communication between mods through shared events and configuration syncing. It also lets users override any changes made by other mods, giving them full control over their settings.

### Why should I use this?

Benefits of using this library:

<details>
    <summary>Configuration Syncing</summary>

This library allows mod pack creators to synchronize configuration files without directly editing them.
Users don’t need to download configuration files from third-party sources.
Configurations can be edited through XML, and the control flow follows this order: User -> Mod -> Cfg Documents<br>

<details>
    <summary>Changing Configuration As User</summary>
    This library includes `PlayerModsOverrides.xml` file that you can open to add or modify XML values.
</details>

<details>
    <summary>Adding Configuration Path To Mod</summary>
    You can add your configuration XML path inside your plugin’s Awake method like this:
    <code>OutwardModsCommunicator.OMC.xmlFilePath = "FullPathTo/MyModsOverrides.xml";</code>
</details>

<details>
    <summary>Editing XML</summary>
    This library will contain `PlayerModsOverrides.xml` document as an example.
    Each override is added inside a ConfigOverrides element:
  <pre><code>&lt;ConfigOverrides&gt;
  &lt;Mod GUID="gymmed.outward_game_settings"&gt;
    &lt;Section Name="Enchanting Modifications"&gt;
      &lt;Entry Key="EnchantingSuccessChance" Value="5" /&gt;
      &lt;Entry Key="RequireRecipeToAllowEnchant" Value="false" /&gt;
      &lt;Entry Key="UseRecipeOnEnchanting" Value="false" /&gt;
    &lt;/Section&gt;
  &lt;/Mod&gt;
  &lt;Mod GUID="gymmed.outward_mods_communicator"&gt;
    &lt;Section Name="Event Profiler"&gt;
      &lt;Entry Key="EnableEventsProfiler" Value="true" /&gt;
      &lt;Entry Key="InstantLogEventsProfileData" Value="true" /&gt;
    &lt;/Section&gt;
  &lt;/Mod&gt;
&lt;/ConfigOverrides&gt;</code></pre>

<details>
    <summary>Possible mod override information can be found in `BepInEx/config` directory inside `.cfg` documents</summary>
<pre><code>## Settings file was created by plugin Outward Game Settings v0.0.2<br>
## Plugin GUID: gymmed.outward_game_settings<br>

[Enchanting Modifications]

\## Allow enchanting only if enchantment is on character?<br>
\# Setting type: Boolean<br>
\# Default value: true<br>
RequireRecipeToAllowEnchant = false<br>

\## Remove recipe after using it on enchanting?<br>
\# Setting type: Boolean<br>
\# Default value: true<br>
UseRecipeOnEnchanting = false<br>

\## What is success chance(%) of enchanting?<br>
\# Setting type: Int32<br>
\# Default value: 50<br>
\# Acceptable value range: From 0 to 100<br>
EnchantingSuccessChance = 5<br>

\## Play additional audio on enchanting failed/success?<br>
\# Setting type: Boolean<br>
\# Default value: true<br>
PlayAudioOnEnchantingDone = true<br></code></pre>
</details>
</details>

</details>

<details>
    <summary>Mods Communication</summary>

Communication is handled through an Event Bus — a system that allows mods to fire (publish) and listen (subscribe) to shared events.

<details>
    <summary>How to register event?</summary>
<pre><code class="language-csharp">using OutwardModsCommunicator.EventBus;
...

void Awake()
{
    ...
    // GUID is your plugin ID provided as a string
    // "EnchantmentMenu@TryEnchant" is the event name
    // ("menu", typeof(EnchantmentMenu)) defines your variable name and its type
    EventBus.RegisterEvent(GUID,  "EnchantmentMenu@TryEnchant", ("menu", typeof(EnchantmentMenu)));

    // you can add multiple variables and
    // add as many as you need like this:
    //EventBus.RegisterEvent("MyPluginId",  "MyClass@MyMethod", ("name", typeof(string)), ("health", typeof(int)));
    ...
}</code></pre>
</details>

<details>
    <summary>How to publish event?</summary>
Use this in places where you want to allow other mods to extend your functionality:
<pre><code class="language-csharp">using OutwardModsCommunicator.EventBus;
...

void YourMethod()
{
    ...
    // Add variable types and names
    var payload = new EventPayload
    {
        ["EnchantmentMenu"] = menu,
    };

    // Send event for subscribers to receive data
    EventBus.Publish(OutwardGameSettings.GUID,  "EnchantmentMenu@TryEnchant", payload);
}</code></pre>
</details>

<details>
    <summary>How to subscribe to event?</summary>
Use this when you want to listen to an event and execute additional code:
<pre><code class="language-csharp">using OutwardModsCommunicator.EventBus;
...

void Awake()
{
    ...
    // Provide other mod GUID
    // Provide event name
    // Provide method you want to execute
    EventBus.Subscribe("gymmed.outward_game_settings", "EnchantmentMenu@TryEnchant", OnTryEnchant);
}

private static void OnTryEnchant(EventPayload payload)
{
    if (payload == null) return;

    // try to retrieve passed event data
    EnchantmentMenu menu = payload.Get<EnchantmentMenu>("menu", null);

    // if event data is null log and stop execution
    if (menu == null)
    {
        Log.LogMessage("Mod gymmed.outward_game_settings event EnchantmentMenu@TryEnchant returned null for EnchantmentMenu");
        return;
    }

    // Lets log success
    Log.LogMessage($"{GUID} successfully communicated with gymmed.outward_game_settings mod and passed menu!");
}</code></pre>
</details>


<details>
    <summary>How to get all registered events?</summary>
Use this when you want to log or inspect all registered events:
<pre><code class="language-csharp">using OutwardModsCommunicator.EventBus;
...

// We use harmony patch to execute code after each plugin is loaded
// and have already registered their events
[HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
public class ResourcesPrefabManager_Load
{
    static void Postfix(ResourcesPrefabManager __instance)
    {
        // Log all registered events
        EventBusDataPresenter.LogRegisteredEvents();
    }
}</code></pre>
</details>
</details>

The project also includes extra tools like `EventProfiler` and `EventBusDataPresenter` — feel free to explore them to learn more.

## Can I find basic example how to use this?

You can view mod creation [template here](https://github.com/GymMed/Outward-Mod-Pack-Template).</br> 
You can view [outward game settings mod here](https://github.com/GymMed/Outward-Game-Settings). 

## How to set up

To manually set up, do the following

1. Create the directory: `Outward\BepInEx\plugins\OutwardModsCommunicator\`.
2. Extract the archive into any directory(recommend empty).
3. Move the contents of the plugins\ directory from the archive into the `BepInEx\plugins\OutwardModsCommunicator\` directory you created.
4. It should look like `Outward\BepInEx\plugins\OutwardModsCommunicator\OutwardModsCommunicator.dll`
   Launch the game, open inventory and view details of item(Equipment or weapon) it should display all available enchantments.

### If you liked the mod leave a star on [GitHub](https://github.com/GymMed/Outward-Mods-Communicator) it's free
