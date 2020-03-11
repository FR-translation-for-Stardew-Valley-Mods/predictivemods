using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class GarbageExperience : Experience
	{
		internal override bool IsAvailable =>
			base.IsAvailable && Garbage.IsAvailable;

		protected override bool Try ()
		{
			// Consume an appropriate offering.
			if (!base.Try () || Offering.Category != StardewValley.Object.junkCategory)
				return false;
			ConsumeOffering ();

			// Show the type menu.
			PlaySound ("trashcan");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (256, 1856, 64, 128), 150f, 6, 1);
			ShowMessage ("garbage.opening", 500);
			Game1.afterDialogues = Run;

			return true;
		}

		public override void Run ()
		{
			// TODO: Show a date selection dialog box.

			Action<WorldDate> callback = (WorldDate date) =>
			{
				// Gather the appropriate predictions.
				List<GarbagePrediction> predictions =
					Garbage.ListLootForDate (date);

				bool today = date == Utilities.Now ();
				List<string> pages = new List<string> ();

				// Show a special message for all cans being empty.
				if (predictions.Count == 0)
				{
					pages.Add (Helper.Translation.Get ($"garbage.none.{(today ? "today" : "later")}"));
				}
				else
				{
					// Build the list of predictions.
					List<string> lines = new List<string>
					{
						Helper.Translation.Get ($"garbage.header.{(today ? "today" : "later")}", new
						{
							date = date.Localize (),
						}),
					};
					foreach (GarbagePrediction prediction in predictions)
					{
						lines.Add (Helper.Translation.Get ($"garbage.prediction.{prediction.Can}", new
						{
							itemName = (prediction.Loot.ParentSheetIndex == 217)
								? Helper.Translation.Get ("garbage.dishOfTheDay")
								: prediction.Loot.DisplayName,
						}));
					};
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
			};

			callback (Utilities.Now ()); // TODO: remove temporary call
		}
	}
}
