using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ScryingOrb
{
	public class Experience
	{
		internal static IModHelper Helper => ModEntry._Helper;

		public static bool Try<T> (StardewValley.Object orb, Item offering)
			where T : Experience, new()
		{
			try
			{
				T experience = new T
				{
					Orb = orb,
					Offering = offering
				};
				return experience.Try ();
			}
			catch (Exception)
			{
				return false;
			}
		}

		protected Experience ()
		{ }

		protected StardewValley.Object Orb { get; private set; }
		protected Item Offering { get; private set; }

		protected virtual bool Try ()
		{
			return Offering != null && (Offering is StardewValley.Object);
		}

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
			DelayedAction.showDialogueAfterDelay (Helper.Translation.Get (messageKey), delay);
		}

		protected void PlaySound (string soundName, int delay = 0)
		{
			DelayedAction.playSoundAfterDelay (soundName, delay,
				Game1.currentLocation);
		}

		protected void ShowAnimation (string textureName,
			Rectangle sourceRect, float interval, int length, int loops,
			int delay = 0)
		{
			Vector2 position = new Vector2 (Orb.TileLocation.X, Orb.TileLocation.Y - 1f);
			position *= Game1.tileSize;
			float layerDepth = (float) (((Orb.TileLocation.Y + 1.0) * 64.0 / 10000.0)
				+ 9.99999974737875E-05);
			TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite
				(textureName, sourceRect, interval, length, loops, position,
				false, false, layerDepth, -0.5f, Color.White, 64f / sourceRect.Width,
				0f, 0f, 0f, false);
			DelayedAction.addTemporarySpriteAfterDelay (sprite,
				Game1.currentLocation, delay);
		}
	}
}