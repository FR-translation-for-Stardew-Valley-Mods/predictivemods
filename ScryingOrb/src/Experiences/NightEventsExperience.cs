using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class NightEventsExperience : Experience
	{
		public static readonly Dictionary<string, int> AcceptedOfferings =
			new Dictionary<string, int>
		{
			{ "Bat Wing", 3 },
			{ "Void Egg", 1 },
			{ "Void Essence", 1 },
			{ "Void Mayonnaise", 1 },
			{ "Void Salmon", 1 },
		};

		public static readonly Dictionary<string, NightEventType?> Types =
			new Dictionary<string, NightEventType?>
		{
			{ "any", null },
			{ "Fairy", NightEventType.Fairy },
			{ "Witch", NightEventType.Witch },
			{ "Meteorite", NightEventType.Meteorite },
			{ "StrangeCapsule", NightEventType.StrangeCapsule },
			{ "StoneOwl", NightEventType.StoneOwl },
			{ "leave", NightEventType.None }
		};

		protected override bool check ()
		{
			// Consume an appropriate offering.
			if (!base.check () || !AcceptedOfferings.ContainsKey (offering.Name))
				return false;
			if (offering.Stack < AcceptedOfferings[offering.Name])
			{
				showRejection ("rejection.insufficient");
				return true;
			}
			consumeOffering (AcceptedOfferings[offering.Name]);

			// React to the offering, then proceed to run.
			illuminate ();
			playSound ("shadowpeep");
			showAnimation ("TileSheets\\animations",
				new Rectangle (0, 2880, 64, 64), 125f, 10, 1);
			showMessage ("nightEvents.opening", 500);
			Game1.afterDialogues = run;

			return true;
		}

		protected override void doRun ()
		{
			// Show the menu of types.
			List<Response> types = Types.Select ((t) => new Response (t.Key,
				Helper.Translation.Get ($"nightEvents.type.{t.Key}"))).ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("nightEvents.type.question"), types);

			Game1.currentLocation.afterQuestion = (Farmer _who, string type) =>
			{
				Game1.currentLocation.afterQuestion = null;

				// If "leave", we're done.
				if (type == "leave")
				{
					extinguish ();
					return;
				}

				// Gather the appropriate predictions.
				List<NightEventPrediction> predictions =
					NightEvents.ListNextEventsForDate (Utilities.Now (), 3,
						Types[type]);
				if (predictions.Count == 0)
				{
					throw new Exception ($"Could not predict night events of {type} type.");
				}

				// Show a list of the predictions.
				List<string> predictionStrings = predictions.Select ((p) =>
					Helper.Translation.Get ($"nightEvents.prediction.{p.type}", new
					{
						date = p.date.Localize (),
					}).ToString ()).ToList ();
				showDialogues (new List<string>
				{
					string.Join ("^", predictionStrings),
					Helper.Translation.Get ("nightEvents.closing")
				});
				Game1.afterDialogues = extinguish;
			};
		}
	}
}
