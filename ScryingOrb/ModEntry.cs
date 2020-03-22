﻿using Microsoft.Xna.Framework;
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

		internal static ModConfig Config;

		private static bool orbHovered;
		internal static bool OrbHovered
		{
			get => orbHovered;
			set
			{
				bool oldValue = orbHovered;
				orbHovered = value;

				// Let the cursor editor know to do its thing.
				if (oldValue != value)
				{
					cursorEditor.Invalidate ();
				}
			}
		}

		private static uint orbsIlluminated;
		internal static uint OrbsIlluminated
		{
			get => orbsIlluminated;
			set
			{
				uint oldValue = orbsIlluminated;
				orbsIlluminated = value;

				// Let the cursor editor know to do its thing.
				if ((oldValue == 0 && value > 0) || (oldValue > 0 && value == 0))
				{
					cursorEditor.Invalidate ();
				}
			}
		}

		private static CursorEditor cursorEditor;

		public override void Entry (IModHelper helper)
		{
			// Read the configuration.
			Config = Helper.ReadConfig<ModConfig> ();

			// Set up PredictiveCore.
			Utilities.Initialize (this, helper);

			// Make resources available.
			_Helper = Helper;
			_Monitor = Monitor;

			// Add console commands.
			Helper.ConsoleCommands.Add ("reset_scrying_orbs",
				"Resets the state of Scrying Orbs to default values.",
				(_command, _args) => ResetScryingOrbs ());
			Helper.ConsoleCommands.Add ("test_scrying_orb",
				"Puts a Scrying Orb and all types of offering into inventory.",
				(_command, _args) => TestScryingOrb ());
			Helper.ConsoleCommands.Add ("test_date_picker",
				"Runs a DatePicker dialog for testing use.",
				(_command, _args) => TestDatePicker ());

			// Listen for game events.
			helper.Events.GameLoop.DayStarted += (_sender, _args) => CheckRecipe ();
			Helper.Events.Input.CursorMoved += OnCursorMoved;
			Helper.Events.Input.ButtonPressed += OnButtonPressed;

			// Set up asset editors.
			cursorEditor = new CursorEditor ();
			Helper.Content.AssetEditors.Add (cursorEditor);
			Helper.Content.AssetEditors.Add (new MailEditor ());
		}

		private void CheckRecipe ()
		{
			// If the recipe is already given, nothing else to do.
			if (Game1.player.craftingRecipes.ContainsKey ("Scrying Orb"))
				return;

			// If the instant recipe cheat is enabled, add the recipe now.
			if (Config.InstantRecipe)
			{
				Game1.player.craftingRecipes.Add ("Scrying Orb", 0);
				return;
			}

			// Otherwise, if the friendship is adequate, deliver the letter.
			if (Game1.player.getFriendshipHeartLevelForNPC ("Wizard") >= 2 &&
					!Game1.player.mailbox.Contains ("kdau.ScryingOrb.welwickInstructions"))
				Game1.player.mailbox.Add ("kdau.ScryingOrb.welwickInstructions");
		}

		private void OnCursorMoved (object sender, CursorMovedEventArgs args)
		{
			// Only hovering if the world is ready and the player is free to
			// interact with an orb.
			if (!Context.IsWorldReady || !Context.IsPlayerFree)
			{
				OrbHovered = false;
				return;
			}

			// Only hovering when a Scrying Orb is pointed at.
			StardewValley.Object orb = Game1.currentLocation.getObjectAtTile
				((int) args.NewPosition.Tile.X, (int) args.NewPosition.Tile.Y);
			OrbHovered = orb != null && orb.Name == "Scrying Orb";
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

			if (Experience.Try<UnlimitedExperience> (orb) ||
				Experience.Try<NothingExperience> (orb) ||
				Experience.Try<LuckyPurpleExperience> (orb) ||
				Experience.Try<MetaExperience> (orb) ||
				Experience.Try<MiningExperience> (orb) ||
				Experience.Try<GeodesExperience> (orb) ||
				Experience.Try<NightEventsExperience> (orb) ||
				// TODO: Experience.Try<ShoppingExperience> (orb) ||
				Experience.Try<GarbageExperience> (orb) ||
				// TODO: Experience.Try<ItemFinderExperience> (orb) ||
				Experience.Try<FallbackExperience> (orb))
			{} // (if-block used to allow boolean fallback)
		}

		private void ResetScryingOrbs ()
		{
			try
			{
				Utilities.CheckWorldReady ();
				UnlimitedExperience.Reset ();
				LuckyPurpleExperience.Reset ();
				MetaExperience.Reset ();
				Monitor.Log ("Scrying Orb state reset to defaults.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Error);
			}
		}

		private void TestScryingOrb ()
		{
			try
			{
				IDictionary<int, string> bci = Game1.bigCraftablesInformation;
				int orbID = bci.First ((kp) => kp.Value.StartsWith ("Scrying Orb/",
					StringComparison.Ordinal)).Key;

				Game1.player.addItemsByMenuIfNecessary (new List<Item>
				{
					new SObject (74, 50), // Prismatic Shard for UnlimitedExperience
					new SObject (789, 1), // Lucky Purple Shorts for LuckyPurpleExperience
					// TODO: item for ItemFinderExperience
					new SObject (168, 150), // 3 Trash for GarbageExperience
					// TODO: item for ShoppingExperience
					new SObject (767, 150), // 3 Bat Wing for NightEventsExperience
					new SObject (541, 50), // Aerinite for GeodesExperience
					new SObject (382, 100), // 2 Coal for MiningExperience
					new SObject (Vector2.Zero, orbID), // Scrying Orb
					new SObject (Vector2.Zero, orbID), // Scrying Orb
					new SObject (Vector2.Zero, orbID) // Scrying Orb
				});

				Monitor.Log ("Scrying Orb test kit placed in inventory.",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Error);
			}
		}

		private void TestDatePicker ()
		{
			try
			{
				WorldDate initialDate = new WorldDate (2, "spring", 15);
				string prompt = "Where on the wheel of the year do you seek?";
				if (Context.IsWorldReady)
					++OrbsIlluminated; // use the special cursor in the dialog
				Game1.activeClickableMenu = new DatePicker (initialDate, prompt,
					(date) =>
					{
						if (Context.IsWorldReady)
							--OrbsIlluminated;
						Monitor.Log ($"DatePicker chose {date}", LogLevel.Info);
					});
			}
			catch (Exception e)
			{
				Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
