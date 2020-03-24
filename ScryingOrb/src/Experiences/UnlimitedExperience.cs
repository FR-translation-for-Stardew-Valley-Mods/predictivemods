using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace ScryingOrb
{
	public class UnlimitedExperience : Experience
	{
		private class Persistent
		{
			public int ExpirationDay { get; set; } = -1;
		}
		private static Persistent persistent;

		public UnlimitedExperience ()
		{
			if (persistent == null)
				persistent = LoadData<Persistent> ("Unlimited");
		}

		public static readonly List<string> AcceptedOfferings = new List<string>
		{
			"Golden Pumpkin",
			"Magic Rock Candy",
			"Pearl",
			"Prismatic Shard",
			"Treasure Chest"
		};

		protected override bool Try ()
		{
			// If currently in an unlimited period, ignore the offering, react
			// to the ongoing period, then proceed to run.
			int totalDays = Utilities.Now ().TotalDays;
			if (totalDays <= persistent.ExpirationDay)
			{
				Illuminate ();
				PlaySound ("yoba");
				ShowMessage ((totalDays == persistent.ExpirationDay)
					? "unlimited.lastDay" : "unlimited.following", 250);
				Game1.afterDialogues = Run;
				return true;
			}

			// Consume an appropriate offering.
			if (!base.Try () ||
					!AcceptedOfferings.Contains (offering.Name))
				return false;
			ConsumeOffering ();

			// Start an unlimited period and increase luck for the day.
			persistent.ExpirationDay = Utilities.Now ().TotalDays +
				(Context.IsMainPlayer ? 7 : 1);
			SaveData ("Unlimited", persistent);
			Game1.player.team.sharedDailyLuck.Value = 0.12;

			// React to the offering dramatically, then proceed to run.
			Illuminate ();
			PlaySound ("reward");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 192, 64, 64), 125f, 8, 1);
			string role = Context.IsMainPlayer ? "main" : "farmhand";
			ShowMessage ($"unlimited.initial.{role}", 1000);
			Game1.afterDialogues = Run;

			return true;
		}

		protected override void DoRun ()
		{
			// In case we were called directly by ModEntry.
			Illuminate ();

			// Show the menu of experiences.
			Dictionary<string, Experience> experiences =
				new Dictionary<string, Experience>
			{
				{ "mining", new MiningExperience { orb = orb } },
				{ "geodes", new GeodesExperience { orb = orb } },
				{ "nightEvents", new NightEventsExperience { orb = orb } },
				// TODO: { "shopping", new ShoppingExperience { Orb = Orb } },
				{ "garbage", new GarbageExperience { orb = orb } },
				// TODO: { "itemFinder", new ItemFinderExperience { Orb = Orb } },
				{ "leave", null }
			};
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
				else
				{
					Extinguish ();
				}
			}, 1);
		}

		internal static void Reset ()
		{
			persistent = new Persistent ();
			SaveData ("Unlimited", persistent);
		}
	}
}
