using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PublicAccessTV
{
	// Container for a pairing of TV message and sprite, allowing changing of
	// scenes between messages.
	internal struct Scene
	{
		public string Message;
		public TemporaryAnimatedSprite Background;
		public TemporaryAnimatedSprite Overlay;
		public Action AfterAction;

		public Scene (string message, TemporaryAnimatedSprite background,
			TemporaryAnimatedSprite overlay = null, Action afterAction = null)
		{
			Message = message;
			Background = background;
			Overlay = overlay;
			AfterAction = afterAction;
		}
	}

	public class Channel
	{
		internal static IModHelper Helper { get => ModEntry._Helper; }
		internal static Type CustomTVMod { get => ModEntry.CustomTVMod; }

		protected readonly string LocalID;
		protected readonly string GlobalID;
		protected readonly string Title;

		protected Channel (string localID)
		{
			LocalID = localID ?? throw new ArgumentNullException (nameof (localID));
			GlobalID = $"kdau.PublicAccessTV.{localID}";
			Title = Helper.Translation.Get ($"{localID}.title");
		}

		// Whether the channel should be available to players at present.
		// TODO: Conditionalize this on the TV subscription.
		internal virtual bool IsAvailable => true;

		// Add or remove the channel based on its availability for the day.
		internal virtual void Initialize ()
		{
			if (IsAvailable)
			{
				Action<TV, TemporaryAnimatedSprite, Farmer, string> action =
					(TV tv, TemporaryAnimatedSprite sprite, Farmer who, string response) => Show (tv);
				CallCustomTVMod ("addChannel", GlobalID, Title, action);
			}
			else
			{
				CallCustomTVMod ("removeChannel", GlobalID);
			}
		}

		// Called by CustomTVMod to start the program. Override to implement.
		internal virtual void Show (TV tv)
		{
			CallCustomTVMod ("showProgram", GlobalID);
		}

		private Queue<Scene> Scenes = new Queue<Scene> ();

		// Add one message-and-sprite pair to the queue for display on TV.
		protected void QueueScene (string message, TemporaryAnimatedSprite background,
			TemporaryAnimatedSprite overlay = null, Action afterAction = null)
		{
			Scenes.Enqueue (new Scene (message, background, overlay, afterAction));
		}

		// Run a program of all the queued scenes on the TV in order.
		protected void RunProgram (TV tv)
		{
			if (Scenes.Count == 0)
			{
				tv.turnOffTV ();
				return;
			}

			Scene scene = Scenes.Dequeue ();
			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screen")
				.SetValue (scene.Background);
			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screenOverlay")
				.SetValue (scene.Overlay);
			Game1.drawObjectDialogue (Game1.parseText (scene.Message));

			Game1.afterDialogues = () =>
			{
				if (scene.AfterAction != null)
				{
					scene.AfterAction.Invoke ();
				}
				RunProgram (tv);
			};
		}

		// Convenience class to handle common configuration needs for TV sprites.
		protected TemporaryAnimatedSprite LoadSprite (TV tv, string textureName,
			Rectangle sourceRect, float animationInterval = 9999f,
			int animationLength = 1, Vector2 positionOffset = new Vector2 (),
			bool overlay = false, bool? scale = null)
		{
			return new TemporaryAnimatedSprite (textureName, sourceRect,
				animationInterval, animationLength, 999999,
				tv.getScreenPosition () + (positionOffset * tv.getScreenSizeModifier ()),
				false, false, (float) (((tv.boundingBox.Bottom - 1) / 10000.0) +
					(overlay ? 1.99999994947575E-05 : 9.99999974737875E-06)),
				0.0f, Color.White, tv.getScreenSizeModifier () *
					((scale ?? !overlay)
						? Math.Min (42f / sourceRect.Width, 28f / sourceRect.Height)
						: 1f),
				0.0f, 0.0f, 0.0f,
				false);
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
