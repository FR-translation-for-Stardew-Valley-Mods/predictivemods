using PredictiveCore;
using StardewModdingAPI;

namespace ScryingOrb
{
	public class ModEntry : Mod
	{
		internal static IModHelper _Helper;

		public override void Entry (IModHelper helper)
		{
			// Set up PredictiveCore.
			Utilities.Initialize (this, helper);

			// Listen for game events.
			_Helper = helper;
			// FIXME: ...
		}
	}
}
