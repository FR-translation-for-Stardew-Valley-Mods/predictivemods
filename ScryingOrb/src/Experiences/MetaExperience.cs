using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ScryingOrb
{
	public class MetaExperience : Experience
	{
		private class Persistent
		{
			public bool AlreadyTried { get; set; } = false;
		}
		private static Persistent persistent;

		public MetaExperience ()
		{
			if (persistent == null)
				persistent = LoadData<Persistent> ("Meta");
		}

		protected override bool check ()
		{
			// Only accept a Scrying Orb. Don't consume it.
			if (!base.check () || !ModEntry.Instance.IsScryingOrb (offering))
				return false;

			// If the player has tried this before, react nonchalantly.
			if (persistent.AlreadyTried)
			{
				playSound ("clank");
				showMessage ("meta.following", 500);
			}
			// Otherwise show the initial joke.
			else
			{
				persistent.AlreadyTried = true;
				SaveData ("Meta", persistent);

				playSound ("clank");
				showMessage ("meta.initial", 500);
			}

			return true;
		}

		internal static void reset ()
		{
			persistent = new Persistent ();
			SaveData ("Meta", persistent);
		}
	}
}
