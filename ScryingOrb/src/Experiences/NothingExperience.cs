using StardewValley;

namespace ScryingOrb
{
	public class NothingExperience : Experience
	{
		protected override bool check ()
		{
			// Unlike all other experiences, look for the offering check to fail.
			if (!isAvailable || base.check ())
				return false;

			showMessage ("rejection.nothing");

			return true;
		}
	}
}
