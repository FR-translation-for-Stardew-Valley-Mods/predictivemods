using StardewValley;

namespace ScryingOrb
{
	public class NothingExperience : Experience
	{
		protected override bool Try (Item offering)
		{
			// Unlike all other experiences, look for the offering check to fail.
			if (!IsAvailable || base.Try (offering))
				return false;

			ShowMessage ("nothing.message");

			return true;
		}
	}
}
