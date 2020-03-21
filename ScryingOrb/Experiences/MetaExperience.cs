using Microsoft.Xna.Framework;
using StardewValley;

namespace ScryingOrb
{
	public class MetaExperience : Experience
	{
		private class SaveData
		{
			public bool AlreadyTried { get; set; } = false;
		}
		private static SaveData saveData;

		public MetaExperience ()
		{
			if (saveData == null)
			{
				saveData = Helper.Data.ReadSaveData<SaveData> ("Meta")
					?? new SaveData ();
			}
		}

		protected override bool Try ()
		{
			// Only accept a Scrying Orb. Don't consume it.
			if (!base.Try () || Offering.Name != "Scrying Orb")
				return false;

			// If the player has tried this before, react nonchalantly.
			if (saveData.AlreadyTried)
			{
				PlaySound ("clank");
				ShowMessage ("meta.following", 500);
			}
			// Otherwise show the initial joke.
			else
			{
				saveData.AlreadyTried = true;
				Helper.Data.WriteSaveData ("Meta", saveData);

				PlaySound ("clank");
				ShowMessage ("meta.initial", 500);
			}

			return true;
		}

		internal static void Reset ()
		{
			Helper.Data.WriteSaveData ("Meta", saveData = new SaveData ());
		}
	}
}
