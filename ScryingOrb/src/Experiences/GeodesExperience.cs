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
			"Prismatic Shard" // accepted by UnlimitedExperience
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
			{ "leave", null }
		};

		public override bool IsAvailable =>
			base.IsAvailable && Geodes.IsAvailable;

		protected override bool Try ()
		{
			// Consume an appropriate offering.
			if (!base.Try () ||
					offering.Category != StardewValley.Object.mineralsCategory ||
					RejectedOfferings.Contains (offering.Name))
				return false;
			ConsumeOffering ();

			// React to the offering, then proceed to run.
			Illuminate ();
			PlaySound ("discoverMineral");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 512, 64, 64), 125f, 8, 1);
			ShowMessage ("geodes.opening", 500);
			Game1.afterDialogues = Run;

			return true;
		}

		protected override void DoRun ()
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
					Extinguish ();
					return;
				}

				// Gather the appropriate predictions.
				List<GeodePrediction> predictions =
				Geodes.ListTreasures (Game1.player.stats.GeodesCracked + 1,
					(type == "any") ? 3u : 10u);
				if (predictions.Count == 0)
				{
					throw new Exception ("Could not predict geode treasures.");
				}

				List<string> pages = new List<string> ();
				string footer = Helper.Translation.Get ("geodes.footer");

				// For the next geode of any type, build a page for each geode
				// with a list of types.
				if (type == "any")
				{
					foreach (GeodePrediction p in predictions)
					{
						uint num = p.number - Game1.player.stats.GeodesCracked;
						string header = Helper.Translation.Get ($"geodes.header.any{num}");
						pages.Add (header + string.Join ("^", p.treasures.Select ((tt) =>
						{
							Treasure t = tt.Value;
							return string.Join (" ", new string[]
							{
								">",
								t.geodeObject.DisplayName + ":",
								(t.stack > 1) ? t.stack.ToString () : null,
								t.displayName,
								t.valuable ? "$" : null,
								t.needDonation ? "=" : null
							}.Where ((s) => s != null));
						})) + footer);
					}
				}
				// For specific types, build a list of geodes.
				else
				{
					string header = Helper.Translation.Get ($"geodes.header.{type}");
					pages.Add (header + string.Join ("^", predictions.Select ((p) =>
					{
						Treasure t = p.treasures[Types[type] ?? GeodeType.Regular];
						uint num = p.number - Game1.player.stats.GeodesCracked;
						return string.Join (" ", new string[]
						{
							string.Format ("{0,2:D}.", num),
							(t.stack > 1) ? t.stack.ToString () : null,
							t.displayName,
							t.valuable ? "$" : null,
							t.needDonation ? "=" : null
						}.Where ((s) => s != null));
					})) + footer);
				}

				// If deeper mine level could alter the results, add an
				// appropriate closing.
				if (Geodes.IsProgressDependent)
				{
					pages.Add (Helper.Translation.Get ("geodes.closing.progress"));
				}

				// Show the predictions.
				ShowDialogues (pages);
				Game1.afterDialogues = Extinguish;
			};
		}
	}
}
