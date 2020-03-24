﻿using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ScryingOrb
{
	public class MiningExperience : Experience
	{
		public static readonly Dictionary<string, int> AcceptedOfferings =
			new Dictionary<string, int>
		{
			{ "Copper Ore", 5 },
			{ "Iron Ore", 3 },
			{ "Gold Ore", 1 },
			{ "Iridium Ore", 1 },
			{ "Coal", 2 },
		};

		public override bool IsAvailable =>
			base.IsAvailable && Mining.IsAvailable;

		protected override bool Try ()
		{
			// Consume an appropriate offering.
			if (!base.Try () || !AcceptedOfferings.ContainsKey (offering.Name))
				return false;
			if (offering.Stack < AcceptedOfferings[offering.Name])
			{
				ShowRejection ("rejection.insufficient");
				return true;
			}
			ConsumeOffering (AcceptedOfferings[offering.Name]);

			// React to the offering, then proceed to run.
			Illuminate ();
			PlaySound ("hammer");
			PlaySound ("stoneCrack");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 3072, 128, 128), 150f, 5, 1);
			ShowMessage ("mining.opening", 500);
			Game1.afterDialogues = Run;

			return true;
		}

		protected override void DoRun ()
		{
			Game1.activeClickableMenu = new DatePicker (Utilities.Now (),
				Helper.Translation.Get ("mining.date.question"), OnDateChosen);
		}
		
		private void OnDateChosen (WorldDate date)
		{
			// Gather the appropriate predictions.
			List<MiningPrediction> predictions =
				Mining.ListFloorsForDate (date);

			bool today = date == Utilities.Now ();
			List<string> pages = new List<string> ();

			// Build the list of predictions.
			List<string> lines = new List<string>
			{
				Helper.Translation.Get ($"mining.header.{(today ? "today" : "later")}", new
				{
					date = date.Localize (),
				})
			};

			string joiner = CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ";
			foreach (MineFloorType type in predictions
				.Select ((p) => p.type).Distinct ().ToList ())
			{
				List<int> floors = predictions
					.Where ((p) => p.type == type)
					.Select ((p) => p.floor)
					.ToList ();
				string floorsText;
				if (floors.Count == 1)
				{
					floorsText = Helper.Translation.Get ("mining.floor",
						new { num = floors[0] });
				}
				else
				{
					int lastNum = floors[floors.Count - 1];
					floors.RemoveAt (floors.Count - 1);
					floorsText = Helper.Translation.Get ("mining.floors",
						new { nums = string.Join (joiner, floors), lastNum = lastNum });
				}

				lines.Add (Helper.Translation.Get ($"mining.prediction.{type}",
					new { floors = floorsText }));
			}
			if (predictions.Count == 0)
				lines.Add (Helper.Translation.Get ("mining.prediction.none"));
			pages.Add (string.Join ("^", lines));

			// If going deeper in the mines could alter the results, add an
			// appropriate closing.
			if (Mining.IsProgressDependent)
			{
				pages.Add (Helper.Translation.Get ("mining.closing.progress"));
			}

			// Show the predictions.
			ShowDialogues (pages);
			Game1.afterDialogues = Extinguish;
		}
	}
}