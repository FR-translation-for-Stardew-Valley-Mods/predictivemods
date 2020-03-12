using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace PublicAccessTV
{
	public class TrainsChannel : Channel
	{
		public TrainsChannel ()
			: base ("trains")
		{
			Helper.Content.Load<Texture2D> ("assets/trains_background.png");
		}

		internal override bool IsAvailable =>
			base.IsAvailable && Trains.IsAvailable &&
			(ModEntry.Config.BypassFriendships || Utilities.AnyoneHasFriendship ("Demetrius", 500));

		internal override void Show (TV tv)
		{
			List<TrainPrediction> predictions =
				Trains.ListNextTrainsForDate (Utilities.Now (), 3);
			if (predictions.Count < 1)
			{
				throw new Exception ("No trains found.");
			}

			TemporaryAnimatedSprite background = LoadSprite (tv,
				Helper.Content.GetActualAssetKey ("assets/trains_background.png"),
				new Rectangle (0, 0, 96, 64));
			TemporaryAnimatedSprite portrait = LoadPortrait (tv, "Demetrius");

			// Opening scene: Demetrius greets the viewer.
			QueueScene (Helper.Translation.Get ("trains.opening"),
				background, portrait);

			// Next scheduled train. Demetrius's reaction depends on whether the
			// train is today, later in the next 7 days, or later than that.
			string nextMessage;
			TemporaryAnimatedSprite nextPortrait;
			WorldDate now = Utilities.Now ();
			if (predictions[0].Date == now)
			{
				nextMessage = "today";
				nextPortrait = LoadPortrait (tv, "Demetrius", 0, 3);
			}
			else if (predictions[0].Date.TotalDays < now.TotalDays + 7)
			{
				nextMessage = "thisWeek";
				nextPortrait = LoadPortrait (tv, "Demetrius", 1, 0);
			}
			else
			{
				nextMessage = "later";
				nextPortrait = LoadPortrait (tv, "Demetrius", 0, 1);
			}
			QueueScene (Helper.Translation.Get ($"trains.next.{nextMessage}", new
				{
					date = predictions[0].Date.Localize (),
					dayOfWeek = predictions[0].Date.DayOfWeek,
					time = Game1.getTimeOfDayString (predictions[0].Time),
				}),
				background, nextPortrait);

			// Second and third scheduled trains.
			if (predictions.Count >= 3)
			{
				QueueScene (Helper.Translation.Get ("trains.later", new
				{
					date1 = predictions[1].Date.Localize (),
					date2 = predictions[2].Date.Localize (),
				}), background, LoadPortrait (tv, "Demetrius", 1, 1));
			}

			// Closing scene: Demetrius signs off.
			QueueScene (Helper.Translation.Get ("trains.closing"),
				background, portrait);

			RunProgram (tv);
		}
	}
}
