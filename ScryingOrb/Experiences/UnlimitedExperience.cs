using Microsoft.Xna.Framework;
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

		public static readonly List<string> Topics = new List<string>
		{
			// TODO: "garbage",
			// TODO: "geodes",
			// TODO: "mining",
			"nightEvents",
			// TODO: "shopping",
			// TODO: "itemFinder",
			"leave",
		};

		protected override bool Try ()
		{
			// If currently in an unlimited period, ignore the offering and show
			// the topic menu.
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

			// Show the topic menu dramatically.
			PlaySound ("reward");
			ShowAnimation ("TileSheets\\animations",
				new Rectangle (0, 192, 64, 64), 125f, 8, 1);
			ShowMessage ("unlimited.initial", 1000);
			Game1.afterDialogues = Run;

			return true;
		}

		public void Run ()
		{
			// Show the menu of topics.
			List<Response> choices = Topics.Select ((t) => new Response (t,
				Helper.Translation.Get ($"unlimited.topic.{t}"))).ToList ();
			Game1.drawObjectQuestionDialogue
				(Helper.Translation.Get ("unlimited.topic.question"), choices);

			// Hand over control to the selected experience. Since that class
			// may also use afterQuestion and uses of it can't be synchronously
			// nested, use a nominal DelayedAction to break out of it.
			Game1.currentLocation.afterQuestion = (Farmer _who, string topic) =>
				DelayedAction.functionAfterDelay (() =>
			{
				Game1.currentLocation.afterQuestion = null;
				switch (topic)
				{
				case "garbage":
					// TODO: new GarbageExperience ().Run ();
					break;
				case "geodes":
					// TODO: new GeodesExperience ().Run ();
					break;
				case "mining":
					// TODO: new MiningExperience ().Run ();
					break;
				case "nightEvents":
					new NightEventsExperience ().Run ();
					break;
				case "shopping":
					// TODO: new ShoppingExperience ().Run ();
					break;
				case "itemFinder":
					// TODO: new ItemFinderExperience ().Run ();
					break;
				case "leave":
					break;
				default:
					throw new ArgumentException ($"Invalid orb topic \"{topic}\" selected.");
				}
			}, 1);
		}

		internal static void Reset ()
		{
			Helper.Data.WriteSaveData ("Unlimited", saveData = new SaveData ());
		}
	}
}
