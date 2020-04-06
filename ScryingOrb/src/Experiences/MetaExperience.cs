using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ScryingOrb
{
	public class MetaExperience : Experience
	{
		private const string TriedFlag =
			"kdau.ScryingOrb.triedMetaOffering";

		protected override bool check ()
		{
			// Only accept a Scrying Orb. Don't consume it.
			if (!base.check () || !ModEntry.Instance.IsScryingOrb (offering))
				return false;

			// If the player has tried this before, react nonchalantly.
			if (Game1.player.mailReceived.Contains (TriedFlag))
			{
				playSound ("clank");
				showMessage ("meta.following", 500);
			}
			// Otherwise show the initial joke.
			else
			{
				Game1.player.mailReceived.Add (TriedFlag);
				playSound ("clank");
				showMessage ("meta.initial", 500);
			}

			return true;
		}

		internal static void Reset ()
		{
			Game1.player.mailReceived.Remove (TriedFlag);
		}
	}
}
