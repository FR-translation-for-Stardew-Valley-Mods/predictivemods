using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using SItem = StardewValley.Item;
using SObject = StardewValley.Object;

namespace PredictiveCore
{
	public enum GarbageCan
	{
		SamHouse, // Jodi, Kent*, Sam, Vincent
		HaleyHouse, // Emily, Haley
		ManorHouse, // Lewis
		ArchaeologyHouse, // Gunther
		Blacksmith, // Clint
		Saloon, // Gus
		JoshHouse, // Alex, Evelyn, George
		JojaMart, // Morris
		MovieTheater, // (only when replaing JojaMart)
	}

	public struct GarbagePrediction
	{
		public WorldDate Date;
		public GarbageCan Can;
		public SItem Loot;
	}

	public static class Garbage
	{
		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Game1.stats.getStat ("trashCansChecked") > 0;

		// Returns the loot to be found in Garbage Cans on the given date.
		public static List<GarbagePrediction> ListLootForDate (WorldDate date)
		{
			Utilities.CheckWorldReady ();

			List<GarbagePrediction> predictions = new List<GarbagePrediction> ();

			foreach (GarbageCan can in Enum.GetValues (typeof (GarbageCan)))
			{
				SItem loot = GetLootForDateAndCan (date, can);
				if (loot != null)
				{
					predictions.Add (new GarbagePrediction
					{
						Date = date,
						Can = can,
						Loot = loot,
					});
				}
			}

			return predictions;
		}

		// Returns whether future progress by the player could alter the result
		// of checking cans. This intentionally disregards the crafting recipes,
		// cooking recipes, mine progress and vault bundle completion considered
		// by Utility.getRandomItemFromSeason, as the combination of those would
		// keep this true well into the midgame.
		public static bool IsProgressDependent
		{
			get
			{
				Utilities.CheckWorldReady ();
				return Game1.stats.getStat ("trashCansChecked") + 1 <= 20;
			}
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
			{
				return;
			}
			Utilities.Helper.ConsoleCommands.Add ("predict_garbage",
				"Predicts the loot to be found in Garbage Cans on a given date, or today by default.\n\nUsage: predict_garbage [<year> <season> <day>]\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				(_command, args) => ConsoleCommand (new List<string> (args)));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				WorldDate date = Utilities.ArgsToWorldDate (args);

				List<GarbagePrediction> predictions = ListLootForDate (date);
				Utilities.Monitor.Log ($"Loot in Garbage Cans on {date}:",
					LogLevel.Info);
				foreach (GarbagePrediction prediction in predictions)
				{
					string name = (prediction.Loot.ParentSheetIndex == 217)
						? "(dish of the day)"
						: prediction.Loot.Name;
					string stars = (prediction.Loot is Hat)
						? " ***"
							: (prediction.Loot is SObject o && o.Flipped)
								? " *"
								: "";
					Utilities.Monitor.Log ($"- {prediction.Can}: {name}{stars}",
						LogLevel.Info);
				}
				if (predictions.Count == 0)
				{
					Utilities.Monitor.Log ("  (none)", LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Alert);
			}
		}

		// The seasonal item roll is affected by the location of the can. The
		// seeds are hardcoded here.
		private static readonly Dictionary<GarbageCan, int> RandomSeedAdditions =
			new Dictionary<GarbageCan, int>
		{
			{ GarbageCan.SamHouse, (13 * 653) + (86 * 777) },
			{ GarbageCan.HaleyHouse, (19 * 653) + (89 * 777) },
			{ GarbageCan.ManorHouse, (56 * 653) + (85 * 777) },
			{ GarbageCan.ArchaeologyHouse, (108 * 653) + (91 * 777) },
			{ GarbageCan.Blacksmith, (97 * 653) + (80 * 777) },
			{ GarbageCan.Saloon, (47 * 653) + (70 * 777) },
			{ GarbageCan.JoshHouse, (52 * 653) + (63 * 777) },
			{ GarbageCan.JojaMart, (110 * 653) + (56 * 777) },
			{ GarbageCan.MovieTheater, (110 * 653) + (56 * 777) },
		};

		private static SItem GetLootForDateAndCan (WorldDate date, GarbageCan can)
		{
			// Logic from StardewValley.Locations.Town.checkAction()
			// as implemented in Stardew Predictor by MouseyPounds.

			// Handle the special case of JojaMart/MovieTheater.
			bool hasTheater = Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccMovieTheater") &&
					!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow ("ccMovieTheaterJoja");
			if ((hasTheater && can == GarbageCan.JojaMart) ||
				(!hasTheater && can == GarbageCan.MovieTheater))
			{
				return null;
			}
			int canValue = (int) ((can == GarbageCan.MovieTheater)
				? GarbageCan.JojaMart : can);

			// Create and prewarm the random generator.
			int daysPlayed = date.TotalDays + 1;
			Random rng = new Random ((int) Game1.uniqueIDForThisGame / 2 +
				daysPlayed + 777 + canValue * 77);
			int prewarm = rng.Next (0, 100);
			for (int i = 0; i < prewarm; i++)
			{
				rng.NextDouble ();
			}
			prewarm = rng.Next (0, 100);
			for (int j = 0; j < prewarm; j++)
			{
				rng.NextDouble ();
			}

			// Roll for regular items.
			uint trashCansChecked = Game1.stats.getStat ("trashCansChecked") + 1;
			bool regular = trashCansChecked > 20 && rng.NextDouble () < 0.01;

			// Roll for the Garbage Hat.
			if (trashCansChecked > 20 && rng.NextDouble () < 0.002)
			{
				return new Hat (66);
			}

			// If the regular roll failed, roll for luck and then give up.
			// Use today's luck for today, else a liquidated value.
			bool today = date == Utilities.Now ();
			double dailyLuck = today ? Game1.player.DailyLuck
				: Game1.player.hasSpecialCharm ? 0.125 : 0.1;
			if (!regular && !(rng.NextDouble () < 0.2 + dailyLuck))
			{
				return null;
			}

			// Roll for a generic or seasonal item.
			int itemID;
			bool seasonal = false;
			switch (rng.Next (10))
			{
			case 1:
				itemID = 167; // Joja Cola
				break;
			case 2:
				itemID = 170; // Broken Glasses
				break;
			case 3:
				itemID = 171; // Broken CD
				break;
			case 4:
				itemID = 172; // Soggy Newspaper
				break;
			case 5:
				itemID = 216; // Bread
				break;
			case 6:
				seasonal = true;
				itemID = Utility.getRandomItemFromSeason (date.Season,
					RandomSeedAdditions[can] + daysPlayed, false, false);
				break;
			case 7:
				itemID = 403; // Field Snack
				break;
			case 8:
				itemID = 309 + rng.Next (3); // Acorn, Maple Seed, Pine Cone
				break;
			case 9:
				itemID = 153; // Green Algae
				break;
			default:
				itemID = 168; // Trash
				break;
			}

			// Roll for location-specific overrides.
			bool locationSpecific = false;
			switch (can)
			{
			case GarbageCan.ArchaeologyHouse:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					if (rng.NextDouble () < 0.05)
					{
						itemID = 749; // Omni Geode
					}
					else
					{
						itemID = 535; // Geode
					}
				}
				break;
			case GarbageCan.Blacksmith:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					itemID = 378 + (rng.Next (3) * 2); // Copper Ore, Iron Ore, Coal
					rng.Next (1, 5); // unused
				}
				break;
			case GarbageCan.Saloon:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					if (!today)
					{
						itemID = 217; // placeholder for dish of the day
					}
					else if (Game1.dishOfTheDay != null)
					{
						itemID = Game1.dishOfTheDay.ParentSheetIndex;
					}
				}
				break;
			case GarbageCan.JoshHouse:
				if (rng.NextDouble () < 0.2 + dailyLuck)
				{
					locationSpecific = true;
					itemID = 223; // Cookie
				}
				break;
			case GarbageCan.JojaMart:
				if (rng.NextDouble () < 0.2 && !Utility.HasAnyPlayerSeenEvent (191393))
				{
					locationSpecific = true;
					itemID = 167; // Joja Cola
				}
				break;
			case GarbageCan.MovieTheater:
				if (rng.NextDouble () < 0.2)
				{
					locationSpecific = true;
					itemID = (rng.NextDouble () < 0.25)
						? 809 : 270; // Movie Ticket, Corn
				}
				break;
			}

			return new SObject (itemID, 1)
			{
				Flipped = seasonal || locationSpecific,
			};
		}
	}
}
