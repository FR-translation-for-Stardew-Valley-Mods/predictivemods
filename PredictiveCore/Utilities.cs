using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;

namespace PredictiveCore
{
	public class WorldUnreadyException : InvalidOperationException
	{
		public WorldUnreadyException ()
			: base ("The world is not ready.")
		{}
	}

	public static class Utilities
	{
		// Parses a list of three console arguments (year, season, day) as a
		// WorldDate. If the list is empty, returns the current date.
		public static WorldDate ArgsToWorldDate (List<string> args)
		{
			switch (args.Count)
			{
			case 0:
				return Now ();
			case 3:
				break;
			default:
				throw new ArgumentException ("Wrong number of arguments.");
			}

			if (!int.TryParse (args[0], out int year) || year < 1)
			{
				throw new ArgumentException ($"Invalid year '{args[0]}', must be a number 1 or higher.");
			}

			string season = args[1];
			if (Utility.getSeasonNumber (season) == -1)
			{
				throw new ArgumentException ($"Invalid season '{args[1]}', must be 'spring', 'summer', 'fall' or 'winter'.");
			}

			if (!int.TryParse (args[2], out int day) || day < 1 || day > 28)
			{
				throw new ArgumentException ($"Invalid day '{args[2]}', must be a number from 1 to 28.");
			}

			return new WorldDate (year, season, day);
		}

		// Converts a TotalDays count to a WorldDate.
		public static WorldDate TotalDaysToWorldDate (int totalDays)
		{
			return new WorldDate () { TotalDays = totalDays };
		}

		// Returns the current WorldDate, since Game1.Date seems to have bugs.
		public static WorldDate Now ()
		{
			return new WorldDate (Game1.year, Game1.currentSeason, Game1.dayOfMonth);
		}

		// Returns the first day of the next season after a given date.
		public static WorldDate GetNextSeasonStart (WorldDate date)
		{
			switch (date.Season)
			{
			case "spring":
				return new WorldDate (date.Year, "summer", 1);
			case "summer":
				return new WorldDate (date.Year, "fall", 1);
			case "fall":
				return new WorldDate (date.Year, "winter", 1);
			default:
				return new WorldDate (date.Year + 1, "spring", 1);
			}
		}

		// Limits prediction calculations to 50 years from start date.
		internal const int MaxHorizon = 28 * 4 * 50;

		internal static IMonitor Monitor;
		internal static IModHelper Helper;

		public static void Initialize (IMod mod, IModHelper helper)
		{
			if (Monitor != null)
				return;
			Monitor = mod.Monitor;
			Helper = helper;

			if (new SemanticVersion (Game1.version).IsOlderThan ("1.4.0") ||
				!new SemanticVersion (Game1.version).IsOlderThan ("1.5.0"))
			{
				Monitor.Log ($"This mod version was not designed for game version {Game1.version}. Predictions will be inaccurate until the mod is updated.", LogLevel.Alert);
			}

			// If multiple mods are consuming PredictiveCore, only add the
			// console commands in one of them (arbitrarily, PublicAccessTV).
			bool addConsoleCommands =
				mod.ModManifest.UniqueID == "kdau.PublicAccessTV" ||
				!helper.ModRegistry.IsLoaded ("kdau.PublicAccessTV");

			Garbage.Initialize (addConsoleCommands);
			Geodes.Initialize (addConsoleCommands);
			// TODO: ItemFinder.Initialize (addConsoleCommands);
			Mining.Initialize (addConsoleCommands);
			Movies.Initialize (addConsoleCommands);
			NightEvents.Initialize (addConsoleCommands);
			// TODO: Shopping.Initialize (addConsoleCommands);
			// TODO: Tailoring.Initialize (addConsoleCommands);
			Trains.Initialize (addConsoleCommands);
		}

		internal static void CheckWorldReady ()
		{
			if (!Context.IsWorldReady)
				throw new WorldUnreadyException ();
		}
	}
}
