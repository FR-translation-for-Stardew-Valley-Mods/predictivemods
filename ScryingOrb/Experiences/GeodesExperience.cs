using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class GeodesExperience : Experience
	{
		public static readonly List<string> RejectedOfferings = new List<string>
		{
			"Limestone", // too cheap
			"Prismatic Shard", // accepted by UnlimitedExperience
		};

		public static readonly Dictionary<string, GeodeType?> Types =
			new Dictionary<string, GeodeType?>
		{
			{ "any", null },
			{ "Regular", GeodeType.Regular },
			{ "Frozen", GeodeType.Frozen },
			{ "Magma", GeodeType.Magma },
			{ "Omni", GeodeType.Omni },
			{ "Trove", GeodeType.Trove },
			{ "leave", null },
		};

		internal override bool IsAvailable =>
			base.IsAvailable && Geodes.IsAvailable;

		protected override bool Try ()
		{
			// Consume an appropriate offering.
			if (!base.Try () || RejectedOfferings.Contains (Offering.Name) ||
					Offering.Category != StardewValley.Object.mineralsCategory)
				return false;
			ConsumeOffering ();

			// Show the type menu.
			PlaySound ("discoverMineral");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 512, 64, 64), 125f, 8, 1);
			ShowMessage ("geodes.opening", 500);
			Game1.afterDialogues = Run;

			return true;
		}

		public override void Run ()
		{
			// Show the menu of types.
			List<Response> types = Types.Select ((t) => new Response (t.Key,
				Helper.Translation.Get ($"geodes.type.{t.Key}"))).ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("geodes.type.question"), types);

			Game1.currentLocation.afterQuestion = (Farmer _who, string type) =>
			{
				Game1.currentLocation.afterQuestion = null;

				// If "leave", we're done.
				if (type == "leave")
				{
					return;
				}

				// Gather the appropriate predictions.
				List<GeodePrediction> predictions =
				Geodes.ListTreasures (Game1.player.stats.GeodesCracked + 1,
					(type == "any") ? 1u : 5u);
				if (predictions.Count == 0)
				{
					throw new Exception ("Could not predict geode treasures.");
				}

				string message;

				// For the next geode of any type, build a list of types.
				if (type == "any")
				{
					message = string.Join ("^", predictions[0].Treasures.Select ((tp) =>
					{
						Treasure t = tp.Value;
						return string.Join (" ", new string[]
						{
							t.GeodeObject.DisplayName + ":",
							(t.Stack > 1) ? t.Stack.ToString () : null,
							t.DisplayName,
							t.Valuable ? "$" : null,
							t.NeedDonation ? "=" : null,
						}.Where ((s) => s != null));
					}));
				}
				// For specific types, build a list of geodes.
				else
				{
					message = string.Join ("^", predictions.Select ((p) =>
					{
						Treasure t = p.Treasures[Types[type] ?? GeodeType.Regular];
						return string.Join (" ", new string[]
						{
							(p.Number - Game1.player.stats.GeodesCracked).ToString () + ".",
							(t.Stack > 1) ? t.Stack.ToString () : null,
							t.DisplayName,
							t.Valuable ? "$" : null,
							t.NeedDonation ? "=" : null,
						}.Where ((s) => s != null));
					}));
				}

				// If deeper mine level could alter the results, add an
				// appropriate closing.
				if (Game1.player.deepestMineLevel <= 75)
				{
					message += "#" + Helper.Translation.Get ("geodes.closing.deeper");
				}

				// Show the predictions.
				Game1.drawObjectDialogue (message);
			};
		}
	}
}
