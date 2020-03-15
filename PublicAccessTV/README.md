# ![[icon]](assets/icon.png) Public Access TV
*a [Stardew Valley](http://stardewvalley.net/) mod by kdau*

Your friends in the valley take to the airwaves to keep you apprised of today's mining conditions, shopping opportunities, garbage loot, train schedules, rare events and more.

This mod is largely based on the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds, whose tools are all worth a look. Its companion mod, Scrying Orb<!-- TODO: ScryingOrb Nexus link --> <small>([source](../ScryingOrb))</small>, offers a different route to some of the same information and more, but looks well beyond the current game day.

## Contents

* [Compatibility](#compatibility)
* [Installation](#installation)
* [Use](#use)
	* [TV schedule](#tv-schedule)
	* [Cheats](#cheats)
* [Translation](#translation)
* [Acknowledgments](#acknowledgments)
* [See also](#see-also)

## Compatibility

Public Access TV is compatible with Stardew Valley 1.4+. There are no known conflicts with other mods, cross-platform issues or problems with multiplayer.

However, some other mods do change the way the game works in the areas that the TV channels cover. The TV will make incorrect announcements in these cases. If you would like me to consider accommodating such a mod, please open an issue on GitLab or on the Nexus page.

## Installation

1. Install the latest version of [SMAPI](https://smapi.io/).
1. Install the [PyTK](https://www.nexusmods.com/stardewvalley/mods/1726) mod.
1. Install this mod from its Nexus page<!-- TODO: PublicAccessTV Nexus link --> or ModDrop page<!-- TODO: PublicAccessTV ModDrop link -->.
1. Run the game using SMAPI.

## Use

In most cases, you will need to befriend the host of a program to a certain number of hearts for them to start broadcasting. Once that has happened, simply turn on any TV and choose the program you want to see. The programs and their requirements are detailed below.

### TV schedule

The standard TV programs, plus any programs added by other mods, will continue to air as they normally do.

#### Governor's Message

The Governor will go live to announce rare events to the entire region. This program is available immediately, but only airs when there is an event to announce. Keep an eye out for it!

<!-- TODO: Mining Advisory -->

<!-- TODO: Shop the Valley -->

#### In the Cans

Hosted by Linus, this daily program clues you in on loot you can find in garbage cans that day. This is a passion project he's sharing with you on a little-known channel. Reach four hearts of friendship with Linus, plus look in at least one can yourself, to inspire him to go on the air.

<!-- TODO: Fashion Showcase -->

#### Train Timetable

Each day, Demetrius comes to you from his lab with a look at the trains scheduled to pass through town soon. Once the railroad is accessible, reach two hearts of friendship with Demetrius and he'll decide to keep his fellow railfans in the loop.

#### Other channels

<details>
<summary>(Spoiler for late-game 1.4 content)</summary>

#### Movie Sneak Preview

Once the movie theater opens, your friendly concessionaire will announce the featured and coming attraction daily. She'll also tip you off on whether there's a line for the ever-popular crane game.
</details>

### Cheats

The first time you run the mod, it will generate a `config.json` file in its main folder (`Mods/PublicAccessTV`). There are no regular options there, but a cheat option is available:

* `BypassFriendships`: Set this to `true` to put all TV hosts on the job as soon as applicable, regardless of their friendship level with the player.

## Translation

This mod can be translated into any language supported by Stardew Valley. No translations are currently available, however.

Your contribution would be welcome. Please see the [details on the wiki](https://stardewvalleywiki.com/Modding:Translations) for help. You can send me your work in an issue on GitLab or Nexus or by DM on Discord.

## Acknowledgments

* Like all mods, this one is indebted to ConcernedApe, particularly for the vanilla assets it adapts.
* The prediction logic behind this mod is largely ported from the [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app by MouseyPounds.
* This mod would not function without [SMAPI](https://smapi.io/) by Pathoschild and [PyTK](https://www.nexusmods.com/stardewvalley/mods/1726) by Platonymous.
* Coding of this mod relied on [Climates of Ferngill](http://www.nexusmods.com/stardewvalley/mods/604) by Sakorona as a key example.
* The #making-mods channel on the [Stardew Valley Discord](https://discordapp.com/invite/StardewValley) offered valuable guidance and feedback.

## See also

* [Release notes](RELEASE-NOTES.md)
* This mod on Nexus<!-- TODO: PublicAccessTV Nexus link -->
* This mod on ModDrop<!-- TODO: PublicAccessTV ModDrop link -->
* [This mod on GitLab](https://gitlab.com/kdau/predictivemods/-/tree/master/PublicAccessTV)
* Scrying Orb<!-- TODO: ScryingOrb Nexus link --> mod <small>([source](../ScryingOrb))</small> for mystical, forward-looking predictions
* [Stardew Predictor](https://mouseypounds.github.io/stardew-predictor/) web app to see it all at once
* [Gardening with Hisame](https://www.nexusmods.com/stardewvalley/mods/5485) mod for another TV program to enjoy
* [Monthly Mail](https://www.nexusmods.com/stardewvalley/mods/4523) mod for more content themed around your game activity
