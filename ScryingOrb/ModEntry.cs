using System;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ScryingOrb
{
	public class ModEntry : Mod
	{
		internal static IModHelper _Helper;

		public override void Entry (IModHelper helper)
		{
			// Set up PredictiveCore.
			Utilities.Initialize (this, helper);

			// Add console commands.
			Utilities.Helper.ConsoleCommands.Add ("reset_orbs",
				"Resets the state of Scrying Orbs to default values.",
				(_command, args) => ResetOrbs ());

			// Listen for game events.
			_Helper = helper;
			Helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		private void OnButtonPressed (object sender, ButtonPressedEventArgs args)
		{
			// Only respond to the action button, and only if the world is ready.
			if (!args.Button.IsActionButton () || !Context.IsWorldReady)
			{
				return;
			}

			// Only respond when a Scrying Orb is interacted with.
			StardewValley.Object orb = Game1.currentLocation.getObjectAtTile
				((int) args.Cursor.GrabTile.X, (int) args.Cursor.GrabTile.Y);
			if (orb == null || orb.Name != "Scrying Orb")
			{
				return;
			}

			// Suppress the button so it won't cause any other effects.
			Helper.Input.Suppress (args.Button);

			Item offering = Game1.player.CurrentItem;

			if (Experience.Try<UnlimitedExperience> (orb, offering) ||
				Experience.Try<NothingExperience> (orb, offering) ||
				Experience.Try<LuckyPurpleExperience> (orb, offering) ||
				// TODO: Experience.Try<GarbageExperience> (orb, offering) ||
				// TODO: Experience.Try<GeodesExperience> (orb, offering) ||
				// TODO: Experience.Try<MiningExperience> (orb, offering) ||
				// TODO: Experience.Try<NightEventsExperience> (orb, offering) ||
				// TODO: Experience.Try<ShoppingExperience> (orb, offering) ||
				// TODO: Experience.Try<ItemFinderExperience> (orb, offering) ||
				Experience.Try<FallbackExperience> (orb, offering))
			{} // (if-block used to allow boolean fallback)
		}

		private void ResetOrbs ()
		{
			try
			{
				Utilities.CheckWorldReady ();
				UnlimitedExperience.Reset ();
				LuckyPurpleExperience.Reset ();
				Monitor.Log ("Scrying Orb state reset to defaults.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Alert);
			}
		}
	}
}
