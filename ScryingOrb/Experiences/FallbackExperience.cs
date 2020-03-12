using StardewValley;

namespace ScryingOrb
{
	public class FallbackExperience : Experience
	{
		protected override bool Try (Item offering)
		{
			// Offering property will not be set.
			PlaySound ("fishEscape");
			ShowMessage ("fallback.message", 250);
			return true;
		}
	}
}
