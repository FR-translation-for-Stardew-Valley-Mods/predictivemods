# ![[icon]](assets/icon.png) Scrying Orb
*a [Stardew Valley](http://stardewvalley.net/) mod by kdau*

Craft a Scrying Orb with a recipe from Welwick and you too can peer into the future. Make an offering to the spirits to scry mining conditions, geode contents, rare events, shopping opportunities or garbage loot.

This mod is based on the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds, whose tools are all worth a look. Its companion mod, Public Access TV<!-- TODO: PublicAccessTV Nexus link --> <small>([source](../PublicAccessTV))</small>, offers a different route to much of the same information for the current game day.

## Contents

* [Compatibility](#compatibility)
* [Installation](#installation)
* [Use](#use)
	* [Offerings](#offerings)
	* [Cheats](#cheats)
* [Translation](#translation)
* [Acknowledgments](#acknowledgments)
* [See also](#see-also)

## Compatibility

Scrying Orb is compatible with Stardew Valley 1.4+. There are no known conflicts with other mods, cross-platform issues or problems with multiplayer.

However, some other mods do change the way the game works in the areas that the Scrying Orb predicts. The orb will make incorrect predictions in these cases. If you would like me to consider accommodating such a mod, please open an issue on GitLab or on the Nexus page.

## Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
1. Install the [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) mod.
1. Optionally, install the [Mail Framework Mod](https://www.nexusmods.com/stardewvalley/mods/1536) to receive the crafting recipe by mail.
1. Install this mod from its Nexus page<!-- TODO: ScryingOrb Nexus link --> or ModDrop page<!-- TODO: ScryingOrb ModDrop link -->.
1. Run the game using SMAPI.

## Use

The day after you reach two hearts of friendship with the Wizard, you will receive a letter in the mail from his colleague Welwick the diviner (from the "Fortune Teller" TV channel). If you already have two or more hearts of friendship with the Wizard, the letter will arrive the first day you play with the mod installed.

Welwick's letter will give you the recipe for crafting the Scrying Orb. Craft the orb with 3 Refined Quartz (for the orb) and 10 Stone (for the pedestal). You can place it anywhere that is convenient, indoors or outdoors. A pickaxe or axe is needed to pick it up again.

To commune with the spirits, offer the orb an item that is symbolic of the topic on which you wish to enquire. Invalid offerings will be rejected without being consumed. A few very special offerings will give you an in-game week of unlimited offering-free scrying.

You can figure out appropriate items as you go, as with gifting. If you're in a hurry, check the topics below to see what is accepted.

### Offerings

Each scrying topic accepts a different set of items.

<!-- TODO: Mines and caverns -->

<details>
<summary>Geode treasures</summary>
(must have cracked at least one geode)

* any Mineral item other than Limestone or Prismatic Shard
</details>

<details>
<summary>Nocturnal occurrences</summary>

* 3 Bat Wing
* Void Egg
* Void Essence
* Void Mayonnaise
* Void Salmon
</details>

<!-- TODO: Market offerings -->

<details>
<summary>Garbage finds</summary>
(must have looked in at least one can)

* any Trash item
</details>

<!-- TODO: Seek a particular item -->

<details>
<summary>Week of unlimited use</summary>

* Golden Pumpkin
* Magic Rock Candy
* Pearl
* Prismatic Shard
* Treasure Chest
</details>

There is also one item which will get a special reaction from the spirits. <span style="color: #9355ea">You probably know <span style="color: #d98cff">which one</span> I mean.</span>

### Cheats

The first time you run the mod, it will generate a `config.json` file in its main folder (`Mods/ScryingOrb`). There are no regular options there, but two cheat options are available:

* `InstantRecipe`: Set this to `true` to enable the Scrying Orb crafting recipe immediately instead of requiring two hearts with the Wizard.
* `UnlimitedUse`: Set this to `true` to have the Scrying Orb work without taking offerings permanently.

## Translation

This mod can be translated into any language supported by Stardew Valley. No translations are currently available, however.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in an issue on GitLab or Nexus or by DM on Discord.

For this mod, in addition to `i18n/default.json`, there are translatable strings in `[JA]ScryingOrb/BigCraftables/ScryingOrb/big-craftable.json` (`Name` and `Description`) and `[MFM]ScryingOrb/i18n/default.json`.

## Acknowledgments

* Like all mods, this one is indebted to ConcernedApe, particularly for the vanilla assets it adapts.
* The prediction logic behind this mod is largely ported from the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds.
* This mod would not function without [SMAPI](https://smapi.io/) by Pathoschild and [Json Assets](https://www.nexusmods.com/stardewvalley/mods/1720) by spacechase0.
* Coding of this mod relied on [Producer Framework Mod](https://www.nexusmods.com/stardewvalley/mods/4970) by Digus as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.
* The date picker is based on a concept from @blueberry on Discord and the aesthetic of [Goethe's color wheel](https://commons.wikimedia.org/wiki/File:Goethe,_Farbenkreis_zur_Symbolisierung_des_menschlichen_Geistes-_und_Seelenlebens,_1809.jpg), with notes from #creative-discussion.

## See also

* [Release notes](RELEASE-NOTES.md)
* This mod on Nexus<!-- TODO: ScryingOrb Nexus link -->
* This mod on ModDrop<!-- TODO: ScryingOrb ModDrop link -->
* [This mod on GitLab](https://gitlab.com/kdau/predictivemods/-/tree/master/ScryingOrb)
* Public Access TV<!-- TODO: PublicAccessTV Nexus link --> mod <small>([source](../PublicAccessTV))</small> for friendship-based, same-day predictions
* [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app to see it all at once
