# ![[icon]](assets/icon.png) Scrying Orb
*a [Stardew Valley](http://stardewvalley.net/) mod by kdau*

Craft a Scrying Orb with a recipe from Welwick and you too can peer into the future. Make an offering to the spirits to scry mining conditions, geode contents, rare events<!-- TODO: , shopping opportunities --> or garbage loot.

This mod is based on the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds, whose tools are all worth a look. Its companion mod, [Public Access TV](https://www.nexusmods.com/stardewvalley/mods/5605) <small>([source](../PublicAccessTV))</small>, offers a different route to much of the same information for the current game day.

## Contents

* [Compatibility](#compatibility)
* [Installation](#installation)
* [Use](#use)
* [Configuration](#configuration)
* [Translation](#translation)
* [Acknowledgments](#acknowledgments)
* [See also](#see-also)

## Compatibility

This version of Scrying Orb is compatible with **Stardew Valley 1.4**. Since it relies heavily on how the game calculates certain events, future updates to the base game are likely to make it inaccurate. When game version 1.5 arrives, take this mod's information with a grain of salt until I have a new version ready.

This mod should work on **Linux, Mac or Windows**. There are no known problems with multiplayer use.

Some other mods change the way the game works in the areas that the Scrying Orb predicts. The following mods are supported:

* [Stardew Valley Expanded](https://www.nexusmods.com/stardewvalley/mods/3753): different arrangement of garbage cans

If any of your other mods affect the areas this mod covers, the orb will make incorrect predictions. If you would like me to consider accommodating such a mod, please open an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues) or [on the Nexus page](https://www.nexusmods.com/stardewvalley/mods/5603?tab=bugs).

## Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
1. Install the [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) mod.
1. Download this mod from its [Nexus page](https://www.nexusmods.com/stardewvalley/mods/5603?tab=files)<!-- TODO: or ModDrop page -->.
1. Unzip the mod and place the `ScryingOrb` and `[JA]ScryingOrb` folders inside your `Mods` folder.
1. Run the game using SMAPI.

## Use

The day after you reach two hearts of friendship with the Wizard, you will receive a letter in the mail from his colleague Welwick the diviner (from the "Fortune Teller" TV channel). If you already have two or more hearts of friendship with the Wizard, the letter will arrive the first day you play with the mod installed.

Welwick's letter will give you the recipe for crafting the Scrying Orb. Craft the orb with 3 Refined Quartz (for the orb) and 10 Stone (for the pedestal). You can place it anywhere that is convenient, indoors or outdoors. A pickaxe or axe is needed to pick it up again.

To commune with the spirits, offer the orb an item that is symbolic of the topic about which you wish to enquire. Invalid offerings will be rejected without being consumed. A few very valuable offerings will give you an (in-game) week of unlimited scrying without further offerings.

You can figure out the appropriate items as you go, like giving gifts. If you're in a hurry, check the topics below to see what is accepted.

### Mines and caverns

Learn what floors of the mines and caverns will have monster infestations or valuable caches on any day in the next year. <em>Must have entered the mines.</em>

<details>
<summary>Offerings</summary>

* 5 Copper Ore
* 3 Iron Ore
* Gold Ore
* Iridium Ore
* 2 Coal
</details>

### Geode treasures

Learn what will be found inside the next three geodes of any type, or the next 10 geodes of a particular type. <em>Must have cracked at least one geode.</em>

<details>
<summary>Offerings</summary>

* any Mineral item other than Limestone or Prismatic Shard
</details>

### Nocturnal occurrences

Learn what strange events may occur over upcoming nights, or when a particular strange event may next occur.

<details>
<summary>Offerings</summary>

* 3 Bat Wing
* Void Egg
* Void Essence
* Void Mayonnaise
* Void Salmon
</details>

<!-- TODO: Market offerings -->

### Garbage finds

Learn what items will be found in garbage cans on any day in the next year. <em>Must have looked in at least one can.</em>

<details>
<summary>Offerings</summary>

* 3 Broken CD
* 3 Broken Glasses
* 3 Driftwood
* 3 Joja Cola
* 3 Rotten Plant
* 3 Soggy Newspaper
* 3 Trash
* *or any combination of the above totaling 3*
</details>

<!-- TODO: Seek a particular item -->

### Week of unlimited use

<details>
<summary>Offerings</summary>

* Golden Pumpkin
* Magic Rock Candy
* Pearl
* Prismatic Shard
* Treasure Chest
</details>

### Other

There is a particular item which will get a special reaction from the spirits. <span style="color: #9355ea">You probably know which one I mean.</span>

## Configuration

The first time you run the mod, it will generate a `config.json` file in its main folder (`Mods/ScryingOrb`). Two cheat options are available:

* `InstantRecipe`: Set this to `true` to give the Scrying Orb crafting recipe immediately instead of needing two hearts with the Wizard.
* `UnlimitedUse`: Set this to `true` to have the Scrying Orb work without taking offerings permanently.

## Translation

This mod can be translated into any language supported by Stardew Valley. No translations are currently available, however.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in an issue [on GitLab](https://gitlab.com/kdau/predictivemods/-/issues), [on Nexus](https://www.nexusmods.com/stardewvalley/mods/5603?tab=bugs) or by DM on Discord.

## Acknowledgments

* Like all mods, this one is indebted to ConcernedApe, particularly for the vanilla assets it adapts.
* The prediction logic behind this mod is largely ported from the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds.
* This mod would not function without [SMAPI](https://smapi.io/) by Pathoschild and [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) by spacechase0.
* Coding of this mod relied on [Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970) by Digus as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* The date picker is based on a concept from @blueberry on Discord and the aesthetic of [Goethe's color wheel](https://commons.wikimedia.org/wiki/File:Goethe,_Farbenkreis_zur_Symbolisierung_des_menschlichen_Geistes-_und_Seelenlebens,_1809.jpg), with notes from #creative-discussion.

## See also

* [Release notes](RELEASE-NOTES.md)
* [This mod on Nexus](https://www.nexusmods.com/stardewvalley/mods/5603)
<!-- TODO: * This mod on ModDrop -->
* [This mod on GitLab](https://gitlab.com/kdau/predictivemods/-/tree/master/ScryingOrb)
---
* [Public Access TV](https://www.nexusmods.com/stardewvalley/mods/5605) mod <small>([source](../PublicAccessTV))</small> for friendship-based, same-day predictions
* [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app to see it all at once
