using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace PublicAccessTV
{
	public class NightEventsChannel : Channel
	{
		public NightEventsChannel ()
			: base ("nightEvents")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "nightEvents_backgrounds.png"));
		}

		internal override bool IsAvailable =>
			base.IsAvailable && NightEvents.IsAvailable &&
			GetCurrentEvent () != NightEventType.None;

		internal override void Show (TV tv)
		{
			NightEventType currentEvent = GetCurrentEvent ();
			if (currentEvent == NightEventType.None)
			{
				throw new Exception ("No night event found.");
			}

			TemporaryAnimatedSprite background = LoadBackground (tv, 0);
			TemporaryAnimatedSprite portrait = LoadPortrait (tv, "Governor", 1, 0);
			bool newYear = currentEvent == NightEventType.NewYear;

			// Opening scene: the governor greets the viewer.
			QueueScene (new Scene
				(Helper.Translation.Get ($"nightEvents.{currentEvent}.opening"),
				background, portrait)
				{ SoundAsset = newYear
					? "nightEvents_newYear" : "nightEvents_opening" });

			// The governor reacts to the event.
			TemporaryAnimatedSprite reactionBackground = background;
			string reactionSound = null;
			Point reactionIndex = new Point (0, newYear ? 0 : 1);
			if (currentEvent == NightEventType.StrangeCapsule)
			{
				reactionBackground = LoadBackground (tv, 0, 1);
				reactionSound = "UFO";
				reactionIndex = new Point (1, 1);
			}
			QueueScene (new Scene (Helper.Translation.Get ($"nightEvents.{currentEvent}.reaction"),
				reactionBackground, LoadPortrait (tv, "Governor", reactionIndex))
				{ SoundCueName = reactionSound });

			// Closing scene: the governor signs off.
			QueueScene (new Scene (Helper.Translation.Get ($"nightEvents.{currentEvent}.closing"),
				background, portrait));

			RunProgram (tv);
		}

		private NightEventType GetCurrentEvent ()
		{
			WorldDate tonight = Utilities.Now ();

			List<NightEventPrediction> predictions =
				NightEvents.ListNextEventsForDate (tonight, 1);
			if (predictions.Count >= 1 && predictions[0].Date == tonight)
			{
				switch (predictions[0].Type)
				{
				case NightEventType.Meteorite:
				case NightEventType.StrangeCapsule:
					return predictions[0].Type;
				}
			}

			if (tonight.Season == "winter" && tonight.DayOfMonth == 28)
			{
				return NightEventType.NewYear;
			}

			return NightEventType.None;
		}
	}
}
