using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class GarbageExperience : Experience
	{
		internal override bool IsAvailable =>
			base.IsAvailable && Garbage.IsAvailable;

		protected override bool Try (Item offering)
		{
			// Consume an appropriate offering.
			if (!base.Try (offering) ||
					Offering.Category != StardewValley.Object.junkCategory)
				return false;
			ConsumeOffering ();

			// React to the offering, then proceed to run.
			Illuminate ();
			PlaySound ("trashcan");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (256, 1856, 64, 128), 150f, 6, 1);
			ShowMessage ("garbage.opening", 500);
			Game1.afterDialogues = Run;

			return true;
		}

		private static readonly string[] Modes =
			new string[] { "today", "later", "hat", "leave" };

		protected override void DoRun ()
		{
			// Show the menu of modes.
			List<Response> modes = Modes.Select ((mode) => new Response (mode,
				Helper.Translation.Get ($"garbage.mode.{mode}"))).ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("garbage.mode.question"), modes);

			Game1.currentLocation.afterQuestion = (Farmer _who, string mode) =>
			{
				Game1.currentLocation.afterQuestion = null;
				WorldDate today = Utilities.Now ();

				switch (mode)
				{
				case "today":
					ShowPredictions (today, Garbage.ListLootForDate (today), mode);
					break;
				case "later":
					Game1.activeClickableMenu = new DatePicker (today,
						Helper.Translation.Get ("garbage.date.question"), (date) =>
							ShowPredictions (date, Garbage.ListLootForDate (date), mode));
					break;
				case "hat":
					GarbagePrediction? hat = Garbage.FindGarbageHat (today);
					List<GarbagePrediction> predictions =
						new List<GarbagePrediction> ();
					if (hat.HasValue)
						predictions.Add (hat.Value);
					ShowPredictions (hat.HasValue ? hat.Value.Date : today,
						predictions, mode);
					break;
				case "leave":
				default:
					Extinguish ();
					break;
				}
			};
		}

		private void ShowPredictions (WorldDate date,
			List<GarbagePrediction> predictions, string mode)
		{
			bool today = date == Utilities.Now ();
			List<string> pages = new List<string> ();

			// Show a special message for all cans being empty.
			if (predictions.Count == 0)
			{
				pages.Add (Helper.Translation.Get ($"garbage.none.{mode}", new
				{
					date = date.Localize (),
				}));
			}
			else
			{
				// Build the list of predictions.
				List<string> lines = new List<string>
				{
					Helper.Translation.Get ($"garbage.header.{(today ? "today" : "later")}", new
					{
						date = date.Localize (),
					})
				};

				// Randomize the order of predictions for variety.
				Random rng = new Random ((int) Game1.uniqueIDForThisGame +
					date.TotalDays);

				foreach (GarbagePrediction prediction in
					predictions.OrderBy ((GarbagePrediction a) => rng.Next ()))
				{
					string can = prediction.Can.ToString ().Replace ("SVE_", "");
					lines.Add (Helper.Translation.Get ($"garbage.prediction.{can}", new
					{
						itemName = (prediction.Loot is Hat)
							? Helper.Translation.Get ("garbage.item.hat")
							: (prediction.Loot.ParentSheetIndex == 217)
								? Helper.Translation.Get ("garbage.item.dishOfTheDay")
								: prediction.Loot.DisplayName,
					}));
				}
				lines.Add (""); // padding for occasional display issues
				pages.Add (string.Join ("^", lines));
			}

			// If checking more cans could alter the results, add an
			// appropriate closing.
			if (Garbage.IsProgressDependent)
			{
				pages.Add (Helper.Translation.Get ("garbage.closing.progress"));
			}

			// Show the predictions.
			ShowDialogues (pages);
			Game1.afterDialogues = Extinguish;
		}
	}
}
