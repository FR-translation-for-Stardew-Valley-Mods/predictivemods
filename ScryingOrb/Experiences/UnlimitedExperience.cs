﻿using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class UnlimitedExperience : Experience
	{
		private class SaveData
		{
			public int ExpirationDay { get; set; } = -1;
		}
		private static SaveData saveData;

		public UnlimitedExperience ()
		{
			if (saveData == null)
			{
				saveData = Helper.Data.ReadSaveData<SaveData> ("Unlimited")
					?? new SaveData ();
			}
		}

		public static readonly List<string> AcceptedOfferings = new List<string>
		{
			"Golden Pumpkin",
			"Magic Rock Candy",
			"Pearl",
			"Prismatic Shard",
			"Treasure Chest",
		};

		private static Dictionary<string, Experience> GetExperiences ()
		{
			return new Dictionary<string, Experience>
			{
				// TODO: { "mining", new MiningExperience () },
				{ "geodes", new GeodesExperience () },
				{ "nightEvents", new NightEventsExperience () },
				// TODO: { "shopping", new ShoppingExperience () },
				// TODO: { "garbage", new GarbageExperience () },
				// TODO: { "itemFinder", new ItemFinderExperience () },
				{ "leave", null },
			};
		}

		protected override bool Try ()
		{
			// If currently in an unlimited period, ignore the offering and show
			// the experience menu.
			if (Utilities.Now ().TotalDays <= saveData.ExpirationDay)
			{
				PlaySound ("yoba");
				ShowMessage ("unlimited.following", 250);
				Game1.afterDialogues = Run;
				return true;
			}

			// Consume an appropriate offering.
			if (!base.Try () || !AcceptedOfferings.Contains (Offering.Name))
				return false;
			ConsumeOffering ();

			// Start an unlimited period.
			saveData.ExpirationDay = Utilities.Now ().TotalDays + 7;
			Helper.Data.WriteSaveData ("Unlimited", saveData);

			// Show the experience menu dramatically.
			PlaySound ("reward");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 192, 64, 64), 125f, 8, 1);
			ShowMessage ("unlimited.initial", 1000);
			Game1.afterDialogues = Run;

			return true;
		}

		public override void Run ()
		{
			// Show the menu of experiences.
			Dictionary<string, Experience> experiences = GetExperiences ();
			List<Response> choices = experiences
				.Where ((e) => e.Value == null || e.Value.IsAvailable)
				.Select ((e) => new Response (e.Key,
					Helper.Translation.Get ($"unlimited.experience.{e.Key}")))
				.ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("unlimited.experience.question"), choices);

			// Hand over control to the selected experience. Since that class
			// may also use afterQuestion and uses of it can't be synchronously
			// nested, use a nominal DelayedAction to break out of it.
			Game1.currentLocation.afterQuestion = (Farmer _who, string response) =>
				DelayedAction.functionAfterDelay (() =>
			{
				Game1.currentLocation.afterQuestion = null;
				Experience experience = experiences[response];
				if (experience != null)
				{
					experience.Run ();
				}
			}, 1);
		}

		internal static void Reset ()
		{
			Helper.Data.WriteSaveData ("Unlimited", saveData = new SaveData ());
		}
	}
}
