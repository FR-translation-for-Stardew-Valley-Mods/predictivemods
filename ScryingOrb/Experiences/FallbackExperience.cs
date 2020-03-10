using System;
using StardewValley;

namespace ScryingOrb
{
	public class FallbackExperience : Experience
	{
		protected override bool Try ()
		{
			PlaySound ("fishEscape");
			ShowMessage ("fallback.message", 250);
			return true;
		}
	}
}
