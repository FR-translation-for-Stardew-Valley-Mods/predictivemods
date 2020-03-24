using StardewValley;

namespace ScryingOrb
{
	public class FallbackExperience : Experience
	{
		protected override bool Try ()
		{
			ShowRejection ("rejection.unrecognized");
			return true;
		}
	}
}
