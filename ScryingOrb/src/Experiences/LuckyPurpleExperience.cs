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

		protected override bool check ()
		{
			// Only accept the Lucky Purple Shorts. Don't consume them.
			if (!base.check () || offering.Name != "Lucky Purple Shorts")
				return false;

			// If the player hasn't tried this before, show the initial warning.
			if (!persistent.AlreadyTried)
			{
				persistent.AlreadyTried = true;
				SaveData ("LuckyPurple", persistent);

				playSound ("grunt");
				showMessage ("luckyPurple.initial", 500);
			}
			// The next time, react dramatically and sour their luck for the day.
			else if (Game1.player.team.sharedDailyLuck.Value > -0.12)
			{
				illuminate (255, 0, 0);
				playSound ("death");
				showAnimation ("TileSheets\\animations",
					new Rectangle (0, 1920, 64, 64), 250f, 4, 2);
				showMessage ("luckyPurple.following", 1000);
				Game1.player.team.sharedDailyLuck.Value = -0.12;
				Game1.afterDialogues = extinguish;
			}
			// But only once a day.
			else
			{
				return false;
			}

			return true;
		}

		internal static void reset ()
		{
			persistent = new Persistent ();
			SaveData ("LuckyPurple", persistent);
		}
	}
}
