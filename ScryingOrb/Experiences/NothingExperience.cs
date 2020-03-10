using System;
using StardewValley;

namespace ScryingOrb
{
	public class NothingExperience : Experience
	{
		protected override bool Try ()
		{
			// Unlike all other experiences, looking for the base check to fail.
			if (base.Try ())
				return false;

			ShowMessage ("nothing.message");

			return true;
		}
	}
}
