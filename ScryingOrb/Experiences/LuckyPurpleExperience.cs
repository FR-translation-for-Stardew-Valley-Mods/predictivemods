using Microsoft.Xna.Framework;
using StardewValley;

namespace ScryingOrb
{
	public class LuckyPurpleExperience : Experience
	{
		private class Persistent
		{
			public bool AlreadyTried { get; set; } = false;
		}
		private static Persistent persistent;

		public LuckyPurpleExperience ()
		{
			if (persistent == null)
				persistent = LoadData<Persistent> ("LuckyPurple");
		}

		protected override bool Try ()
		{
			// Only accept the Lucky Purple Shorts. Don't consume them.
			if (!base.Try () || Offering.Name != "Lucky Purple Shorts")
				return false;

			// If the player hasn't tried this before, show the initial warning.
			if (!persistent.AlreadyTried)
			{
				persistent.AlreadyTried = true;
				SaveData ("LuckyPurple", persistent);

				PlaySound ("grunt");
				ShowMessage ("luckyPurple.initial", 500);
			}
			// The next time, react dramatically and sour their luck for the day.
			else if (Game1.player.team.sharedDailyLuck.Value > -0.12)
			{
				Illuminate (255, 0, 0);
				PlaySound ("death");
				ShowAnimation ("TileSheets\\animations",
					new Rectangle (0, 1920, 64, 64), 250f, 4, 2);
				ShowMessage ("luckyPurple.following", 1000);
				Game1.player.team.sharedDailyLuck.Value = -0.12;
				Game1.afterDialogues = Extinguish;
			}
			// But only once a day.
			else
			{
				return false;
			}

			return true;
		}

		internal static void Reset ()
		{
			persistent = new Persistent ();
			SaveData ("LuckyPurple", persistent);
		}
	}
}
