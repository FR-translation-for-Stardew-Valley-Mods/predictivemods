using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace PublicAccessTV
{
	internal class ModConfig
	{
		public bool BypassFriendships { get; set; } = false;
	}

	public class ModEntry : Mod
	{
		internal static IModHelper _Helper;
		internal static Type CustomTVMod;
		internal static ModConfig Config;
		internal Channel[] Channels;

		public override void Entry (IModHelper helper)
		{
			// Read the configuration.
			Config = this.Helper.ReadConfig<ModConfig> ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, helper);

			// Add console commands.
			Utilities.Helper.ConsoleCommands.Add ("update_patv_channels",
				"Updates the availability of the custom channels to reflect current conditions.",
				(_command, args) => UpdateChannels ());

			// Listen for game events.
			_Helper = helper;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted += (_sender, _args) => InitializeChannels ();
		}

		private void OnGameLaunched (object sender, GameLaunchedEventArgs e)
		{
			// Access CustomTVMod in PyTK. Using reflection to work around the
			// base game's cross-platform assembly name inconsistency.
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
				new NightEventsChannel (),
				new MiningChannel (),
				// TODO: new ShoppingChannel (),
				new GarbageChannel (),
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

		private void UpdateChannels ()
		{
			try
			{
				Utilities.CheckWorldReady ();
				InitializeChannels ();
				Monitor.Log ("Channel availability updated to reflect current conditions.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
