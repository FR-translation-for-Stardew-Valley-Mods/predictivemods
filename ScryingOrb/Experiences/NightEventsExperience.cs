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

		public static readonly Dictionary<string, NightEventType?> Subtopics =
			new Dictionary<string, NightEventType?>
		{
			{ "upcoming", null },
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

			// Show the subtopic menu.
			PlaySound ("shadowpeep");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 2880, 64, 64), 125f, 10, 1);
			ShowMessage ("nightEvents.opening", 500);
			Game1.afterDialogues = Run;

			return true;
		}

		public void Run ()
		{
			// Show the menu of subtopics.
			List<Response> choices = Subtopics.Select ((s) => new Response (s.Key,
				Helper.Translation.Get ($"nightEvents.subtopic.{s.Key}"))).ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("nightEvents.subtopic.question"), choices);

			Game1.currentLocation.afterQuestion = (Farmer _who, string subtopic) =>
			{
				Game1.currentLocation.afterQuestion = null;

				// If "leave", we're done.
				if (subtopic == "leave")
				{
					return;
				}

				// Gather the appropriate predictions.
				List<NightEventPrediction> predictions =
					NightEvents.ListNextEventsForDate (Utilities.Now (), 3,
						Subtopics[subtopic]);
				if (predictions.Count == 0)
				{
					throw new Exception ("Could not predict night events.");
				}

				// Show a list of the predictions.
				List<string> messages = predictions.Select ((p) =>
					Helper.Translation.Get ($"nightEvents.prediction.{p.Type}", new
					{
						date = p.Date.Localize (),
					}).ToString ()).ToList ();
				Game1.drawObjectDialogue (string.Join ("^", messages));

				// Show closing message.
				Game1.afterDialogues = () => ShowMessage ("nightEvents.closing");
			};
		}
	}
}
