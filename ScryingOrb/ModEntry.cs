using Microsoft.Xna.Framework;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	internal class ModConfig
	{
		public bool InstantRecipe { get; set; } = false;
		public bool UnlimitedUse { get; set; } = false;
	}

	public class ModEntry : Mod
	{
		internal static IModHelper _Helper;
		internal static IMonitor _Monitor;
		private ModConfig Config;

		public override void Entry (IModHelper helper)
		{
			// Read the configuration.
			this.Config = this.Helper.ReadConfig<ModConfig> ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, helper);

			// Add console commands.
			Utilities.Helper.ConsoleCommands.Add ("reset_orbs",
				"Resets the state of Scrying Orbs to default values.",
				(_command, args) => ResetOrbs ());
			Utilities.Helper.ConsoleCommands.Add ("orb_test_kit",
				"Puts a Scrying Orb and all types of offering into inventory.",
				(_command, args) => OrbTestKit ());

			// Listen for game events.
			_Helper = helper;
			_Monitor = Monitor;
			helper.Events.GameLoop.DayStarted += (_sender, _args) => CheckRecipe ();
			Helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		private void CheckRecipe ()
		{
			// If the instant recipe cheat is enabled, add the recipe now.
			if (Config.InstantRecipe &&
				!Game1.player.craftingRecipes.ContainsKey ("Scrying Orb"))
			{
				Game1.player.craftingRecipes.Add ("Scrying Orb", 0);
			}
		}

		private void OnButtonPressed (object sender, ButtonPressedEventArgs args)
		{
			// Only respond to the action button, and only if the world is ready
			// and the player is free to interact with an orb.
			if (!Context.IsWorldReady || !Context.IsPlayerFree ||
				!args.Button.IsActionButton ())
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

			// If the unlimited use cheat is enabled, skip to the menu.
			if (Config.UnlimitedUse)
			{
				Experience.Run<UnlimitedExperience> (orb);
				return;
			}

			Item offering = Game1.player.CurrentItem;

			if (Experience.Try<UnlimitedExperience> (orb, offering) ||
				Experience.Try<NothingExperience> (orb, offering) ||
				Experience.Try<LuckyPurpleExperience> (orb, offering) ||
				// TODO: Experience.Try<MiningExperience> (orb, offering) ||
				Experience.Try<GeodesExperience> (orb, offering) ||
				Experience.Try<NightEventsExperience> (orb, offering) ||
				// TODO: Experience.Try<ShoppingExperience> (orb, offering) ||
				Experience.Try<GarbageExperience> (orb, offering) ||
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

		private void OrbTestKit ()
		{
			try
			{
				IDictionary<int, string> bci = Game1.bigCraftablesInformation;
				int orbID = bci.First ((kp) => kp.Value.StartsWith ("Scrying Orb/",
					StringComparison.Ordinal)).Key;

				Game1.player.addItemsByMenuIfNecessary (new List<Item>
				{
					new SObject (Vector2.Zero, orbID), // Scrying Orb
					new SObject (Vector2.Zero, orbID), // Scrying Orb
					new SObject (Vector2.Zero, orbID), // Scrying Orb
					// TODO: item for MiningExperience
					new SObject (541, 50), // Aerinite for GeodesExperience
					new SObject (767, 150), // 3 Bat Wing for NightEventsExperience
					// TODO: item for ShoppingExperience
					new SObject (168, 50), // Trash for GarbageExperience
					// TODO: item for ItemFinderExperience
					new SObject (789, 1), // Lucky Purple Shorts for LuckyPurpleExperience
					new SObject (74, 50), // Prismatic Shard for UnlimitedExperience
				});
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Alert);
			}
		}
	}
}
