using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public class GarbageExperience : Experience
	{
		public override bool IsAvailable =>
			base.IsAvailable && Garbage.IsAvailable;

		protected override bool Try ()
		{
			// Require an appropriate offering.
			if (!base.Try () ||
					offering.Category != StardewValley.Object.junkCategory)
				return false;
			
			// Consume a total of 3 trash, combining across stacks in inventory.
			Queue<SObject> offerings = new Queue<SObject> ();
			offerings.Enqueue (offering);
			int stack = Math.Min (3, offering.Stack);
			foreach (Item item in Game1.player.items)
			{
				if (stack == 3)
					break;
				if (!(item is SObject obj) || object.ReferenceEquals (obj, offering))
					continue;
				if (obj.Category != StardewValley.Object.junkCategory)
					continue;
				offerings.Enqueue (obj);
				stack += Math.Min (3 - stack, obj.Stack);
			}
			if (stack < 3)
			{
				ShowRejection ("rejection.insufficient");
				return true;
			}
			while (stack > 0 && offerings.Count > 0)
			{
				SObject offering = offerings.Dequeue ();
				int count = Math.Min (stack, offering.Stack);
				ConsumeOffering (count, offering);
				stack -= count;
			}

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
					ShowPredictions (hat.HasValue ? hat.Value.date : today,
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
					string can = prediction.can.ToString ().Replace ("SVE_", "");
					lines.Add (Helper.Translation.Get ($"garbage.prediction.{can}", new
					{
						itemName = (prediction.loot is Hat)
							? Helper.Translation.Get ("garbage.item.hat")
							: (prediction.loot.ParentSheetIndex == 217)
								? Helper.Translation.Get ("garbage.item.dishOfTheDay")
								: prediction.loot.DisplayName,
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
