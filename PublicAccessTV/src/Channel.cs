﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PublicAccessTV
{
	public class Channel
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;
		protected static Type CustomTVMod => ModEntry.CustomTVMod;

		public readonly string localID;
		public readonly string globalID;

		public string title => Helper.Translation.Get ($"{localID}.title");

		private readonly Action<TV, TemporaryAnimatedSprite, Farmer, string> callback;

		protected Channel (string localID)
		{
			this.localID = localID ?? throw new ArgumentNullException (nameof (localID));
			globalID = $"kdau.PublicAccessTV.{localID}";
			callback = (tv, _sprite, _who, _response) => Show (tv);
			CallCustomTVMod ("addChannel", globalID, title, callback);
		}

		// Whether the channel should be available to players at present.
		internal virtual bool IsAvailable => true;

		// Add or remove the channel based on its availability for the day.
		internal virtual void Update ()
		{
			if (IsAvailable)
			{
				CallCustomTVMod ("addChannel", globalID, title, callback);
			}
			else
			{
				CallCustomTVMod ("removeChannel", globalID);
			}
		}

		// Reset any persistent state for the channel.
		internal virtual void Reset ()
		{}

		// Called by CustomTVMod to start the program. Override to implement.
		internal virtual void Show (TV tv)
		{
			CallCustomTVMod ("showProgram", globalID);
		}

		private Queue<Scene> Scenes = new Queue<Scene> ();

		// Add a scene to the queue for display on TV.
		protected void QueueScene (Scene scene)
		{
			Scenes.Enqueue (scene);
		}

		// Run a program of all the queued scenes on the TV in order.
		public void RunProgram (TV tv)
		{
			if (Scenes.Count == 0)
			{
				tv.turnOffTV ();
				Game1.stopMusicTrack (Game1.MusicContext.Event);
			}
			else
			{
				Scene scene = Scenes.Dequeue ();
				scene.Run (tv, this);
			}
		}

		// Convenience method to handle common values for TV sprites.
		protected TemporaryAnimatedSprite LoadSprite (TV tv, string textureName,
			Rectangle sourceRect, float animationInterval = 9999f,
			int animationLength = 1, Vector2 positionOffset = new Vector2 (),
			bool overlay = false, bool? scaleToFit = null, float extraScale = 1f)
		{
			float scale = extraScale *
				((scaleToFit ?? !overlay)
					? Math.Min (42f / sourceRect.Width, 28f / sourceRect.Height)
				: 1f);
			float layerDepth = (float) (((tv.boundingBox.Bottom - 1) / 10000.0) +
				(overlay ? 1.99999994947575E-05 : 9.99999974737875E-06));
			return new TemporaryAnimatedSprite (textureName, sourceRect,
				animationInterval, animationLength, 999999,
				tv.getScreenPosition () + (positionOffset * tv.getScreenSizeModifier ()),
				false, false, layerDepth, 0f, Color.White,
				tv.getScreenSizeModifier () * scale, 0f, 0f, 0f, false);
		}

		// Convenience method for background TV sprites from tilesheets.
		protected TemporaryAnimatedSprite LoadBackground (TV tv, int scene,
			int condition = 0)
		{
			return LoadSprite (tv,
				Helper.Content.GetActualAssetKey
					(Path.Combine ("assets", $"{localID}_backgrounds.png")),
				new Rectangle (condition * 120, scene * 80, 120, 80));
		}

		// Convenience method for portrait overlay TV sprites.
		protected TemporaryAnimatedSprite LoadPortrait (TV tv, string npc,
			int xIndex, int yIndex)
		{
			return LoadSprite (tv, $"Portraits\\{npc}",
				new Rectangle (xIndex * 64, yIndex * 64, 64, 64),
				positionOffset: new Vector2 (17.5f, 3.5f), overlay: true,
				scaleToFit: true, extraScale: 0.875f);
		}
		protected TemporaryAnimatedSprite LoadPortrait (TV tv, string npc,
			Point? index = null)
		{
			Point _index = index ?? new Point (0, 0);
			return LoadPortrait (tv, npc, _index.X, _index.Y);
		}

		protected void CallCustomTVMod (string methodName, params object[] arguments)
		{
			CallCustomTVModTyped (methodName, arguments,
				arguments.Select (arg => arg.GetType ()).ToArray ());
		}

		private void CallCustomTVModTyped (string methodName, object[] arguments, Type[] types)
		{
			MethodInfo method = CustomTVMod.GetMethod (methodName, types)
				?? throw new NotImplementedException ($"CustomTVMod.{methodName} not found.");
			method.Invoke (null, arguments);
		}
	}
}
