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
		private static readonly List<string> Seasons = new List<string> (new[] { "spring", "summer", "fall", "winter" });

		// Parses a series of three console arguments (Y M D) as a WorldDate.
		public static WorldDate ArgsToWorldDate (string[] args)
		{
			if (args.Length != 3)
			{
				throw new ArgumentException ("Wrong number of arguments.");
			}

			if (!int.TryParse (args[0], out int year) || year < 1)
			{
				throw new ArgumentException ($"Invalid year '{args[0]}', must be a number 1 or higher.");
			}

			string season = args[1];
			if (!Seasons.Contains (season))
			{
				throw new ArgumentException ($"Invalid season '{args[1]}', must be 'spring', 'summer', 'fall' or 'winter'.");
			}

			if (!int.TryParse (args[2], out int day) || day < 1 || day > 28)
			{
				throw new ArgumentException ($"Invalid day '{args[2]}', must be a number from 1 to 28.");
			}

			return new WorldDate (year, season, day);
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

		// Returns whether the farmer or any farmhand has a friendship of at
		// least the given level with the named NPC.
		public static bool AnyoneHasFriendship (string npcName, int level)
		{
			foreach (Farmer farmer in Game1.getAllFarmers ())
			{
				if (farmer.getFriendshipLevelForNPC (npcName) >= level)
				{
					return true;
				}
			}
			return false;
		}

		internal static IMonitor Monitor;
		internal static IModHelper Helper;

		public static void Initialize (IMod mod, IModHelper helper)
		{
			if (Monitor != null)
			{
				return;
			}
			Monitor = mod.Monitor;
			Helper = helper;

			Movies.Initialize ();
		}

		internal static void CheckWorldReady ()
		{
			if (!Context.IsWorldReady)
			{
				throw new WorldUnreadyException ();
			}
		}
	}
}
