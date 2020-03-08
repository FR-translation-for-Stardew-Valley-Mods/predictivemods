using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace PublicAccessTV
{
	public class ModEntry : Mod
	{
		internal static IModHelper _Helper;
		internal static Type CustomTVMod;
		internal Channel[] Channels;

		public override void Entry (IModHelper helper)
		{
			// Set up PredictiveCore.
			Utilities.Initialize (this, helper);

			// Listen for game events.
			_Helper = helper;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
		}

		private void OnGameLaunched (object sender, GameLaunchedEventArgs e)
		{
			// Use reflection to access CustomTVMod in PyTK, to work around
			// a cross-platform problem when developing from Linux.
			CustomTVMod = Helper.ModRegistry.GetApi ("Platonymous.Toolkit")
				?.GetType ()?.Assembly?.GetType ("PyTK.CustomTV.CustomTVMod");
			if (CustomTVMod == null)
			{
				Monitor.Log ("PyTK's CustomTVMod not found, so cannot create TV channels.",
					LogLevel.Alert);
				return;
			}

			// Create the channels.
			Channels = new[]
			{
				// TODO: new GarbageChannel ()
				// TODO: new MiningChannel ()
				new MoviesChannel ()
				// TODO: new NightEventsChannel ()
				// TODO: new ShoppingChannel ()
				// TODO: new TailoringChannel ()
				// TODO: new TrainsChannel ()
			};
		}

		private void OnDayStarted (object sender, DayStartedEventArgs e)
		{
			// Set up each channel for the day.
			foreach (Channel channel in Channels)
			{
				channel.Initialize ();
			}
		}
	}
}
