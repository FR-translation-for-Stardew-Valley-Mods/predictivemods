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

			// Add console commands.
			Utilities.Helper.ConsoleCommands.Add ("reset_channels",
				"Resets the availability of the custom channels to reflect current conditions.",
				(_command, args) =>
				{
					InitializeChannels ();
					Monitor.Log ("Channel availability reset to reflect current conditions.",
						LogLevel.Info);
				});

			// Listen for game events.
			_Helper = helper;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted += (_sender, _args) => InitializeChannels ();
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
			Channels = new Channel[]
			{
				// TODO: new NightEventsChannel (),
				// TODO: new MiningChannel (),
				// TODO: new ShoppingChannel (),
				// TODO: new GarbageChannel (),
				// TODO: new TailoringChannel (),
				new TrainsChannel (),
				new MoviesChannel ()
			};
		}

		private void InitializeChannels ()
		{
			// Initialize each channel based on current conditions.
			foreach (Channel channel in Channels)
			{
				channel.Initialize ();
			}
		}
	}
}
