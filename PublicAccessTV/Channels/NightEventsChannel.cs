using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;

namespace PublicAccessTV
{
	public class NightEventsChannel : Channel
	{
		public NightEventsChannel ()
			: base ("nightEvents")
		{
			Helper.Content.Load<Texture2D> ("assets/nightEvents_background.png");
		}

		internal override bool IsAvailable => base.IsAvailable &&
			NightEvents.IsAvailable && GetCurrentEvent () != NightEventType.None;

		internal override void Show (TV tv)
		{
			NightEventType currentEvent = GetCurrentEvent ();
			if (currentEvent == NightEventType.None)
			{
				throw new Exception ("No night event found.");
			}

			TemporaryAnimatedSprite background = LoadSprite (tv,
				Helper.Content.GetActualAssetKey ("assets/nightEvents_background.png"),
				new Rectangle (0, 0, 120, 80));
			TemporaryAnimatedSprite portrait = LoadPortrait (tv, "Governor", 1, 0);

			// Opening scene: the governor greets the viewer.
			QueueScene (Helper.Translation.Get ($"nightEvents.{currentEvent}.opening"),
				background, portrait);

			// The governor reacts to the event.
			Point reactionIndex;
			switch (currentEvent)
			{
			case NightEventType.StrangeCapsule:
				reactionIndex = new Point (1, 1);
				break;
			case NightEventType.NewYear:
				reactionIndex = new Point (0, 0);
				break;
			default:
				reactionIndex = new Point (0, 1);
				break;
			}
			QueueScene (Helper.Translation.Get ($"nightEvents.{currentEvent}.reaction"),
				background, LoadPortrait (tv, "Governor", reactionIndex));

			// Closing scene: the governor signs off.
			QueueScene (Helper.Translation.Get ($"nightEvents.{currentEvent}.closing"),
				background, portrait);

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
