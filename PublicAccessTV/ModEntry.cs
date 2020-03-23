using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace PublicAccessTV
{
	internal class ModConfig : IConfig
	{
		public bool BypassFriendships { get; set; } = false;
		public bool InaccuratePredictions { get; set; } = false;
	}

	public class ModEntry : Mod
	{
		internal static IModHelper _Helper;
		internal static IMonitor _Monitor;
		internal static Type CustomTVMod;
		internal static ModConfig Config;

		internal Channel[] channels;

		public override void Entry (IModHelper helper)
		{
			// Read the configuration.
			Config = Helper.ReadConfig<ModConfig> ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, Config);

			// Make resources available.
			_Helper = Helper;
			_Monitor = Monitor;

			// Add console commands.
			Helper.ConsoleCommands.Add ("update_patv_channels",
				"Updates the availability of the custom channels to reflect current conditions.",
				(_command, args) => UpdateChannels (true));
			Helper.ConsoleCommands.Add ("reset_patv_channels",
				"Resets the custom channels to their unlaunched states (before letters, events, etc.).",
				(_command, args) => ResetChannels (true));

			// Listen for game events.
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.DayStarted +=
				(_sender, _args) => UpdateChannels ();
			helper.Events.GameLoop.OneSecondUpdateTicked +=
				(_sender, _args) => GarbageChannel.CheckEvent ();

			// Set up asset editors.
			Helper.Content.AssetEditors.Add (new DialogueEditor ());
			Helper.Content.AssetEditors.Add (new EventsEditor ());
			Helper.Content.AssetEditors.Add (new MailEditor ());
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
			channels = new Channel[]
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

		private void UpdateChannels (bool isCommand = false)
		{
			try
			{
				Utilities.CheckWorldReady ();
				foreach (Channel channel in channels)
					channel.Update ();
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Error);
			}
			if (isCommand)
			{
				Monitor.Log ("Channel availability updated to reflect current conditions.",
					LogLevel.Info);
			}
		}

		private void ResetChannels (bool isCommand = false)
		{
			try
			{
				Utilities.CheckWorldReady ();
				foreach (Channel channel in channels)
					channel.Reset ();
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Error);
			}
			if (isCommand)
			{
				Monitor.Log ("Channels reset to initial states.",
					LogLevel.Info);
			}
		}
	}
}
