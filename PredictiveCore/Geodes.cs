using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace PredictiveCore
{
	public enum GeodeType
	{
		Regular,
		Frozen,
		Magma,
		Omni,
		Trove,
	}

	public class Treasure
	{
		public static readonly List<string> UndonatableTreasures = new List<string>
		{
			"Coal",
			"Clay",
			"Stone",
			"Copper Ore",
			"Iron Ore",
			"Gold Ore",
			"Iridium Ore",
			"Golden Pumpkin",
			"Treasure Chest",
			"Pearl",
		};

		private static readonly Dictionary<GeodeType, int> GeodeObjects =
			new Dictionary<GeodeType, int>
		{
			{ GeodeType.Regular, 535 },
			{ GeodeType.Frozen, 536 },
			{ GeodeType.Magma, 537 },
			{ GeodeType.Omni, 749 },
			{ GeodeType.Trove, 275 },
		};

		internal Treasure (uint geodeNumber, GeodeType geodeType)
		{
			GeodeNumber = geodeNumber;
			GeodeType = geodeType;
			GeodeObject = new SObject (GeodeObjects[geodeType], 1);

			uint originalNumber = Game1.player.stats.GeodesCracked;
			try
			{
				Game1.player.stats.GeodesCracked = geodeNumber;
				Item = Utility.getTreasureFromGeode (GeodeObject);
			}
			finally
			{
				Game1.player.stats.GeodesCracked = originalNumber;
			}
		}

		public readonly uint GeodeNumber;
		public readonly GeodeType GeodeType;
		public readonly SObject GeodeObject;

		public readonly SObject Item;
		public int Stack => Item.Stack;
		public string DisplayName => Item.DisplayName;

		public bool Valuable => Item.Stack * Item.Price > 75;
		public bool NeedDonation => !UndonatableTreasures.Contains (Item.Name) &&
			!new LibraryMuseum ().museumAlreadyHasArtifact (Item.ParentSheetIndex);
	}

	public class GeodePrediction
	{
		public readonly uint Number;
		public readonly SortedDictionary<GeodeType, Treasure> Treasures;

		internal GeodePrediction (uint number)
		{
			Number = number;
			Treasures = new SortedDictionary<GeodeType, Treasure> ();
			foreach (GeodeType type in new GeodeType[]
				{ GeodeType.Regular, GeodeType.Frozen, GeodeType.Magma, GeodeType.Omni, GeodeType.Trove })
			{
				Treasures.Add (type, new Treasure (number, type));
			}
		}
	}

	public static class Geodes
	{
		// Whether this module should be available for player use.
		public static bool IsAvailable => Game1.player.stats.GeodesCracked > 0;

		// Returns the treasures in the next several geodes to be cracked on or
		// after the given geode number, up to the given limit.
		public static List<GeodePrediction> ListTreasures
			(uint fromNumber, uint limit)
		{
			Utilities.CheckWorldReady ();

			List<GeodePrediction> predictions = new List<GeodePrediction> ();

			for (uint number = fromNumber; number < fromNumber + limit; ++number)
			{
				predictions.Add (new GeodePrediction (number));
			}

			return predictions;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
			{
				return;
			}
			Utilities.Helper.ConsoleCommands.Add ("predict_geodes",
				"Predicts the treasures in the next several geodes to be cracked on or after a given geode, or the next geodes by default.\n\nUsage: predict_geodes [<limit> [<number>]]\n- limit: number of treasures to predict (default 20)\n- number: the number of geodes cracked after the first treasure to predict (a number starting from 1).",
				(_command, args) => ConsoleCommand (new List<string> (args)));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				Utilities.CheckWorldReady ();

				uint limit = 20;
				if (args.Count > 0)
				{
					if (!uint.TryParse (args[0], out limit) || limit < 1)
					{
						throw new ArgumentException ($"Invalid limit '{args[0]}', must be a number 1 or higher.");
					}
					args.RemoveAt (0);
				}

				uint number = Game1.player.stats.GeodesCracked + 1;
				if (args.Count > 0)
				{
					if (!uint.TryParse (args[0], out number) || number < 1)
					{
						throw new ArgumentException ($"Invalid geode number '{args[0]}', must be a number 1 or higher.");
					}
					args.RemoveAt (0);
				}
				WorldDate date = Utilities.ArgsToWorldDate (args);

				List<GeodePrediction> predictions = ListTreasures (number, limit);
				Utilities.Monitor.Log ($"Next {limit} treasure(s) starting with geode {number}:",
					LogLevel.Info);
				Utilities.Monitor.Log ("  (*: museum donation needed; $: valuable)",
					LogLevel.Info);
				Utilities.Monitor.Log ("Number | Geode                  *$ | Frozen Geode           *$ | Magma Geode            *$ | Omni Geode             *$ | Artifact Trove         *$",
					LogLevel.Info);
				Utilities.Monitor.Log ("-------|---------------------------|---------------------------|---------------------------|---------------------------|--------------------------",
					LogLevel.Info);
				foreach (GeodePrediction prediction in predictions)
				{
					string treasures = string.Join (" | ",
						prediction.Treasures.Values.Select ((t) =>
							string.Format ("{0,2:D} {1,-20}{2}{3}",
								t.Stack, t.DisplayName,
								t.NeedDonation ? "*" : " ",
								t.Valuable ? "$" : " "
							)));
					Utilities.Monitor.Log (string.Format ("{0,5:D}. | {1}",
						prediction.Number, treasures), LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Alert);
			}
		}
	}
}
