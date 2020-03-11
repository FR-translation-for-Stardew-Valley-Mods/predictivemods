using Microsoft.Xna.Framework;

namespace ScryingOrb
{
	public class LuckyPurpleExperience : Experience
	{
		private class SaveData
		{
			public bool AlreadyTried { get; set; } = false;
		}
		private static SaveData saveData;

		public LuckyPurpleExperience ()
		{
			if (saveData == null)
			{
				saveData = Helper.Data.ReadSaveData<SaveData> ("LuckyPurple")
					?? new SaveData ();
			}
		}

		protected override bool Try ()
		{
			if (!base.Try ())
				return false;

			// Only accept the Lucky Purple Shorts. Don't consume them.
			if (Offering.Name != "Lucky Purple Shorts")
			{
				return false;
			}

			// If the player has tried this before, react nonchalantly.
			if (saveData.AlreadyTried)
			{
				PlaySound ("grunt");
				ShowMessage ("luckyPurple.following", 500);
			}
			// Otherwise react dramatically.
			else
			{
				saveData.AlreadyTried = true;
				Helper.Data.WriteSaveData ("LuckyPurple", saveData);

				PlaySound ("death");
				ShowAnimation ("TileSheets\\animations",
					new Rectangle (0, 1920, 64, 64), 250f, 4, 2);
				ShowMessage ("luckyPurple.initial", 1000);
			}

			return true;
		}

		internal static void Reset ()
		{
			Helper.Data.WriteSaveData ("LuckyPurple", saveData = new SaveData ());
		}
	}
}
