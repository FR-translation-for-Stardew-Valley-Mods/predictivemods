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
		public static readonly List<string> AcceptedOfferings = new List<string>
		{
			"Bat Wing", // 3x
			"Void Egg",
			"Void Essence",
			"Void Mayonnaise",
			"Void Salmon",
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
			{ "leave", NightEventType.None },
		};

		protected override bool Try ()
		{
			// Consume an appropriate offering.
			if (!base.Try () || !AcceptedOfferings.Contains (Offering.Name) ||
					(Offering.Name == "Bat Wing" && Offering.Stack < 3))
				return false;
			ConsumeOffering ((Offering.Name == "Bat Wing") ? 3 : 1);

			// Show the type menu.
			PlaySound ("shadowpeep");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 2880, 64, 64), 125f, 10, 1);
			ShowMessage ("nightEvents.opening", 500);
			Game1.afterDialogues = Run;

			return true;
		}

		public override void Run ()
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
				List<string> messages = predictions.Select ((p) =>
					Helper.Translation.Get ($"nightEvents.prediction.{p.Type}", new
					{
						date = p.Date.Localize (),
					}).ToString ()).ToList ();
				Game1.drawObjectDialogue (string.Join ("^", messages) + "#" +
					Helper.Translation.Get ("nightEvents.closing"));
			};
		}
	}
}
