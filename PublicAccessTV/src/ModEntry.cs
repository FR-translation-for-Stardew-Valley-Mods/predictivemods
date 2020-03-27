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
		internal static ModEntry Instance { get; private set; }
		internal static Type CustomTVMod { get; private set; }
		internal static ModConfig Config { get; private set; }

		internal Channel[] channels { get; private set; }

		public override void Entry (IModHelper helper)
		{
			// Make resources available.
			Instance = this;
			Config = Helper.ReadConfig<ModConfig> ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, Config);

			// Set up asset editors.
			Helper.Content.AssetEditors.Add (new DialogueEditor ());
			Helper.Content.AssetEditors.Add (new EventsEditor ());
			Helper.Content.AssetEditors.Add (new MailEditor ());

			// Add console commands.
			Helper.ConsoleCommands.Add ("update_patv_channels",
				"Updates the availability of the custom channels to reflect current conditions.",
				(_command, _args) => updateChannels (true));
			Helper.ConsoleCommands.Add ("reset_patv_channels",
				"Resets the custom channels to their unlaunched states (before letters, events, etc.).",
				cmdResetChannels);

			// Listen for game events.
			Helper.Events.GameLoop.GameLaunched += onGameLaunched;
			Helper.Events.GameLoop.DayStarted +=
				(_sender, _e) => updateChannels ();
			Helper.Events.GameLoop.OneSecondUpdateTicked +=
				(_sender, _e) => GarbageChannel.CheckEvent ();
		}

		private void onGameLaunched (object _sender, GameLaunchedEventArgs _e)
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

		private void updateChannels (bool isCommand = false)
		{
			try
			{
				Utilities.CheckWorldReady ();
				foreach (Channel channel in channels)
					channel.update ();
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

		private void cmdResetChannels (string _command, string[] _args)
		{
			try
			{
				Utilities.CheckWorldReady ();
				foreach (Channel channel in channels)
					channel.reset ();
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Error);
			}
			Monitor.Log ("Channels reset to initial states.",
				LogLevel.Info);
		}
	}
}
