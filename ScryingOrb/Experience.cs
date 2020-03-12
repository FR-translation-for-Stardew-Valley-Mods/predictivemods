﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace ScryingOrb
{
	public class Experience
	{
		internal static IModHelper Helper => ModEntry._Helper;
		internal static IMonitor Monitor => ModEntry._Monitor;

		// Whether the experience should be available to players at present.
		internal virtual bool IsAvailable => true;

		public static bool Try<T> (SObject orb, Item offering)
			where T : Experience, new()
		{
			try
			{
				T experience = new T { Orb = orb };
				return experience.Try (offering);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static void Run<T> (SObject orb)
			where T : Experience, new()
		{
			T experience = new T { Orb = orb };
			experience.Run ();
		}

		protected Experience ()
		{
			Helper.Content.Load<Texture2D> ("assets/illumination.png");
		}

		public SObject Orb { get; internal set; }
		public SObject Offering { get; internal set; }

		protected virtual bool Try (Item offering)
		{
			if (!IsAvailable)
			{
				return false;
			}
			if (!(offering is SObject o))
			{
				return false;
			}
			Offering = o;
			return true;
		}

		public virtual void Run ()
		{}

		protected void ConsumeOffering (int count = 1)
		{
			if (Offering == null)
			{
				throw new NullReferenceException ("No offering is available to be consumed.");
			}

			if (Offering.Stack > count)
			{
				Offering.Stack -= count;
			}
			else if (Offering.Stack == count)
			{
				Game1.player.removeItemFromInventory (Offering);
			}
			else
			{
				throw new ArgumentOutOfRangeException ($"Offering stack of {Offering.Stack} insufficient for count of {count}.");
			}
		}

		protected void ShowMessage (string messageKey, int delay = 0)
		{
			ShowDialogues (new List<string> { Helper.Translation.Get (messageKey) },
				delay);
		}

		protected void ShowDialogues (List<string> dialogues, int delay = 0)
		{
			// Equivalent to DelayedAction.showDialogueAfterDelay combined with
			// Game1.drawDialogueNoTyping, except that the player is locked
			// instantly before the delay and the List<string> constructor of
			// DialogueBox is used to work around the height estimation bug
			// with multi-page dialogues.

			// Pad each line of each page with a space to work around a width
			// estimation bug in the List-based DialogueBox. *sigh*
			for (int i = 0; i < dialogues.Count; ++i)
			{
				dialogues[i] = dialogues[i].Replace ("^", " ^") + " ";
			}

			// Prepare the UI and player before the delay.
			if (Game1.activeClickableMenu != null)
			{
				Game1.activeClickableMenu.emergencyShutDown ();
			}
			Game1.player.CanMove = false;
			Game1.dialogueUp = true;

			DelayedAction.functionAfterDelay (() =>
			{
				// Display the dialogues.
				Game1.activeClickableMenu = new DialogueBox (dialogues);

				// Suppress typing of dialogue, at least on first page.
				if (Game1.activeClickableMenu != null &&
					Game1.activeClickableMenu is DialogueBox dialogueBox)
				{
					dialogueBox.finishTyping ();
				}
			}, delay);
		}

		protected void PlaySound (string soundName, int delay = 0)
		{
			DelayedAction.playSoundAfterDelay (soundName, delay,
				Game1.currentLocation);
		}

		protected TemporaryAnimatedSprite ShowAnimation (string textureName,
			Rectangle sourceRect, float interval, int length, int loops,
			int delay = 0)
		{
			Vector2 position = new Vector2 (Orb.TileLocation.X,
				Orb.TileLocation.Y - (sourceRect.Height / (float) sourceRect.Width));
			position *= Game1.tileSize;
			float layerDepth = (float) (((Orb.TileLocation.Y + 1.0) * 64.0 / 10000.0)
				+ 9.99999974737875E-05);
			TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite
				(textureName, sourceRect, interval, length, loops, position,
				false, false, layerDepth, 0f, Color.White, 64f / sourceRect.Width,
				0f, 0f, 0f, false);
			DelayedAction.addTemporarySpriteAfterDelay (sprite,
				Game1.currentLocation, delay);
			return sprite;
		}

		protected LightSource Illuminate (int r = 153, int g = 217, int b = 234)
		{
			if (Orb == null)
			{
				return null;
			}

			// Replace any existing light source.
			Extinguish ();

			// Calculate the light source properties.
			Vector2 position = new Vector2 ((Orb.TileLocation.X * 64f) + 32f,
				(Orb.TileLocation.Y * 64f) - 32f);
			Color color = new Color (255 - r, 255 - g, 255 - b) * 2f;
			int identifier = (int) ((Orb.TileLocation.X * 2000f) +
				Orb.TileLocation.Y);

			// Switch the orb to its illuminated sprite, unless not lit blue.
			if (b > r && b > g)
			{
				TemporaryAnimatedSprite sprite = ShowAnimation
					(Helper.Content.GetActualAssetKey ("assets/illumination.png"),
					new Rectangle (0, 0, 16, 16), 150f, 5, 9999);
				sprite.id = identifier;
			}
			
			// Construct and apply the light source.
			Orb.lightSource = new LightSource (LightSource.cauldronLight,
				position, 1f, color, identifier);
			return Orb.lightSource;
		}

		protected void Extinguish ()
		{
			if (Orb == null || Orb.lightSource == null)
			{
				return;
			}
			Game1.currentLocation.removeTemporarySpritesWithID (Orb.lightSource.Identifier);
			Game1.currentLocation.removeLightSource (Orb.lightSource.Identifier);
			Orb.lightSource = null;
		}
	}
}
