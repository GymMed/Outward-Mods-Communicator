<h1 align="center">
    Outward Mods Communicator
</h1>
<br/>
<div align="center">
  <img src="https://raw.githubusercontent.com/GymMed/Outward-Mods-Communicator/refs/heads/main/preview/images/Logo.png" alt="Outward game setting to require enchantment recipe when enchanting."/>
</div>

<div align="center">
	<a href="https://thunderstore.io/c/outward/p/GymMed/Mods_Communicator/">
		<img src="https://img.shields.io/thunderstore/dt/GymMed/Mods_Communicator" alt="Thunderstore Downloads">
	</a>
	<a href="https://github.com/GymMed/Outward-Mods-Communicator/releases/latest">
		<img src="https://img.shields.io/thunderstore/v/GymMed/Mods_Communicator" alt="Thunderstore Version">
	</a>
</div>

Outward Mods Communicator enables seamless communication between mods through shared events and configuration syncing. It also lets users override any changes made by other mods, giving them full control over their settings. You can find library published on NuGet as `Outward.ModsCommunicator`.

### Why should I use this?

Benefits of using this library:

<details>
    <summary>Configuration Syncing</summary>

This library allows mod pack creators to synchronize configuration files without directly editing them.
Users don’t need to download configuration files from third-party sources.
Configurations can be edited through XML, and the control flow follows this order: User -> Mod -> Cfg Documents<br>

<details>
    <summary>Deeper explanation</summary>
Currently, Outward's BepInEx does not provide a built-in way for mods to modify 
<code>.cfg</code> configuration files inside the <code>BepInEx/config</code> directory.  
Mods Communicator solves this by using two XML files. One XML file is placed in the 
mod’s plugin directory (recommended), and Mods Communicator uses it to override 
configuration settings right after the <code>ResourcesPrefabManager</code> class 
finishes its <code>Load</code> method.  
Afterwards, it reads the player’s <code>PlayerModsOverrides.xml</code> file, located 
in <code>BepInEx/config/gymmed.Mods_Communicator</code> folder, to apply personal overrides based on the player’s preferences.  

<h3>FAQ</h3>

<details>
    <summary>Why use XML instead of hardcoding values in the assembly/code/DLL?</summary>
    XML is used so players can freely edit values without needing to modify any code.  
    The XML structure is simple, human-readable, and gives full control to the user.
</details>

<details>
    <summary>Why are there two XML files?</summary>
    Mods can be updated frequently, and each update may include new default XML values.  
    By separating the player’s personal overrides into a second XML file, updates won’t 
    overwrite their custom settings.  
    Mods Communicator only provides a <code>BepInEx/config/gymmed.Mods_Communicator/PlayerModsOverrides.example.xml</code> file — 
    players must rename it to <code>BepInEx/config/gymmed.Mods_Communicator/PlayerModsOverrides.xml</code>.  
    This makes their configuration safe from updates and re-downloads.
</details>

<details>
    <summary>Why are XML files read after the <code>ResourcesPrefabManager.Load</code> method?</summary>
    Because all plugins and their configurations must be initialized first.  
    Once <code>ResourcesPrefabManager.Load</code> completes, all 
    <code>.cfg</code> files are loaded, and it becomes safe to apply overrides.
</details>

<details>
    <summary>Does every mod that changes settings through configs need Mods Communicator as a dependency?</summary>
    No. The Mods Communicator library is only required for mods that want to modify 
    other mods’ default values to create new experiences.  
    For example, the <code>Outward Game Settings</code> mod uses this dependency 
    solely for the <code>EventBus</code> feature to publish additional events.
</details>
</details>

<details>
    <summary>Changing Configuration As User</summary>
This library includes <code>PlayerModsOverrides.example.xml</code> file
inside your <code>BepInEx/config/gymmed.Mods_Communicator</code> folder.
You will need to change it's name to <code>PlayerModsOverrides.xml</code>
when you can open it to add or modify XML values. </details>

<details>
    <summary>Adding Configuration Path To Mod</summary>
    You can add your configuration XML path inside your plugin’s Awake method like this:
    <code>OutwardModsCommunicator.OMC.xmlFilePath = "FullPathTo/MyModsOverrides.xml";</code>
</details>

<details>
    <summary>Editing XML</summary>
    This library will contain <code>BepInEx/config/gymmed.Mods_Communicator/PlayerModsOverrides.example.xml</code> document as an example.
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
    <summary>Possible mod override information can be found in <code>BepInEx/config</code> directory inside <code>.cfg</code> documents</summary>
<pre><code>## Settings file was created by plugin Outward Game Settings v0.0.1<br>
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
  <summary>Deeper Explanation</summary>
  <p> Outward&#x2019;s BepInEx framework uses <b>Harmony</b> for patching &#x2014; allowing mods to modify or extend existing game logic. However, Harmony alone cannot handle <b>communication between mods</b>. That&#x2019;s where the <b>Event Bus</b> comes in. </p>
  <p> Harmony and the Event Bus serve <b>different purposes</b>: </p>
  <details>
    <summary>Harmony &#x2014; Modify Existing Behavior</summary>
    <p>
  Harmony is a patching library that injects code before, after, or inside existing game methods.
  It&#x2019;s great for changing how things work but not for sharing data between mods.
</p>
    <ul>
      <li>
        <b>Pros:</b>
      </li>
      <ul>
        <li>Perfect for tweaking existing systems (combat, AI, loot, etc.).</li>
        <li>Gives direct control over the game&#x2019;s original methods.</li>
      </ul>
      <li>
        <b>Cons:</b>
      </li>
      <ul>
        <li>Can only modify what already exists.</li>
        <li>Requires DLL references to patch another mod&#x2019;s code.</li>
        <li>Breaks easily when mods or the game update.</li>
        <li>Cannot define new, reusable communication points for others.</li>
      </ul>
    </ul>
    <p>
      <i>Harmony changes how things behave &#x2014; not how mods talk to each other.</i>
    </p>
  </details>
  <details>
    <summary>Event Bus &#x2014; Enable Mod Communication</summary>
    <p>
  The Event Bus allows mods to <b>publish and subscribe</b> to events without referencing each
  other&#x2019;s DLLs. It introduces a safe, modular way for mods to share information and react to
  in-game events.
</p>
    <ul>
      <li>
        <b>Advantages:</b>
      </li>
      <ul>
        <li>No fragile DLL dependencies or version mismatches.</li>
        <li>Mods stay modular &#x2014; one mod can publish, others can listen.</li>
        <li>Supports entirely new custom events that didn&#x2019;t exist before.</li>
      </ul>
    </ul>
    <p><b>Example:</b>
  A <a href="https://github.com/GymMed/Outward-Game-Settings">Game Settings mod</a> can publish an <code>OnEnchantSuccess</code> event.  
  Another mod can listen for it and play new sounds, change visuals,
  or trigger additional effects when enchantments succeed.
</p>
  </details>
  <h3>FAQ</h3>
  <details>
    <summary>Can&#x2019;t I just patch another mod&#x2019;s method?</summary>
    <p> You can &#x2014; but it&#x2019;s fragile. You&#x2019;ll need to include that mod&#x2019;s DLL as a dependency, and any update to it can break your patch. The <b>Mods Communicator</b> Event Bus avoids this problem &#x2014; mods only publish and subscribe to shared events, keeping them safe and compatible. </p>
  </details>
  <details>
    <summary>Can I create libraries with Mods Communicator?</summary>
    <p> Yes. You can create lightweight library mods that listen for published events and transform complex logic into simpler, abstracted event calls. Example: The <a href="https://github.com/GymMed/Outward-Loot-Manager">Loot Manager</a> listens for death events and injects new loot. </p>
  </details>
  <details>
    <summary>How does this help implement new features?</summary>
    <p> The <a href="https://github.com/GymMed/Outward-Game-Settings">Game Settings Mod</a> adds enchantment success chance and exposes success/failure events. Other mods can subscribe to these to: </p>
    <ul>
      <li>Disable default enchantment sounds.</li>
      <li>Play new audio clips or effects.</li>
      <li>Trigger visual cues or animations on success or failure.</li>
    </ul>
  </details>
  <p><b>Summary:</b> Harmony modifies existing game behavior. Event Bus enables safe, modular mod communication. Together, they make Outward&#x2019;s modding ecosystem more powerful and extensible. </p>
</details>

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
    // add optional description about parameter(is variable optional?)
    //EventBus.RegisterEvent(GUID,  "EnchantmentMenu@TryEnchant", ("menu", typeof(EnchantmentMenu), "The enchantment menu instance that invoked the TryEnchant method."));
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
    // Add variable receiver names and your variables references
    var payload = new EventPayload
    {
        // Will get it as named menu
        ["menu"] = yourVariable,
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
        // log received payload for errors inspection
        EventBusDataPresenter.LogPayload(payload);
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
        // Log all subsribers
        EventBusDataPresenter.LogAllModsSubsribers();
    }
}</code></pre>
</details>
</details>

The project also includes extra tools like `EventProfiler` and `EventBusDataPresenter` — feel free to explore them to learn more.

## Can I find basic example how to use this?

You can view mod creation [template here](https://github.com/GymMed/Outward-Mod-Pack-Template).<br>
You can view [outward game settings mod here](https://github.com/GymMed/Outward-Game-Settings).<br>
You can view more complex example [outward loot manager mod here](https://github.com/GymMed/Outward-Loot-Manager).

## How to set up

To manually set up, do the following

1. Create the directory: `Outward\BepInEx\plugins\OutwardModsCommunicator\`.
2. Extract the archive into any directory(recommend empty).
3. Move the contents of the plugins\ directory from the archive into the `BepInEx\plugins\OutwardModsCommunicator\` directory you created.
4. It should look like `Outward\BepInEx\plugins\OutwardModsCommunicator\OutwardModsCommunicator.dll`
   Launch the game.

### If you liked the mod leave a star on [GitHub](https://github.com/GymMed/Outward-Mods-Communicator) it's free
