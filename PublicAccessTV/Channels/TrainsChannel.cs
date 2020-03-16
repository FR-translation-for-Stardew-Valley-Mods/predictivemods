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
	public class TrainsChannel : Channel
	{
		public TrainsChannel ()
			: base ("trains")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "trains_backgrounds.png"));
		}

		internal override bool IsAvailable =>
			base.IsAvailable && Trains.IsAvailable &&
			(ModEntry.Config.BypassFriendships ||
				Game1.player.getFriendshipHeartLevelForNPC ("Demetrius") >= 2);

		internal override void Show (TV tv)
		{
			List<TrainPrediction> predictions =
				Trains.ListNextTrainsForDate (Utilities.Now (), 3);
			if (predictions.Count < 1)
			{
				throw new Exception ("No trains found.");
			}

			TemporaryAnimatedSprite background = LoadBackground (tv, 0,
				(Game1.isRaining || Game1.isDarkOut ()) ? 1 : 0);
			TemporaryAnimatedSprite portrait = LoadPortrait (tv, "Demetrius");

			// Opening scene: Demetrius greets the viewer.
			QueueScene (new Scene (Helper.Translation.Get ("trains.opening"),
				background, portrait) { SoundAsset = "trains_opening" });

			// Next scheduled train. Demetrius's reaction depends on whether the
			// train is today, later in the next 7 days, or later than that.
			string nextMessage;
			TemporaryAnimatedSprite nextPortrait;
			string nextSound = null;
			WorldDate now = Utilities.Now ();
			if (predictions[0].Date == now)
			{
				nextMessage = "today";
				nextPortrait = LoadPortrait (tv, "Demetrius", 0, 3);
				nextSound = "trainWhistle";
			}
			else if (predictions[0].Date.TotalDays < now.TotalDays + 7)
			{
				nextMessage = "thisWeek";
				nextPortrait = LoadPortrait (tv, "Demetrius", 1, 0);
				nextSound = "distantTrain";
			}
			else
			{
				nextMessage = "later";
				nextPortrait = LoadPortrait (tv, "Demetrius", 0, 1);
			}
			QueueScene (new Scene (Helper.Translation.Get ($"trains.next.{nextMessage}", new
				{
					date = predictions[0].Date.Localize (),
					dayOfWeek = predictions[0].Date.DayOfWeek,
					time = Game1.getTimeOfDayString (predictions[0].Time),
				}),
				background, nextPortrait) { SoundCueName = nextSound });

			// Second and third scheduled trains.
			if (predictions.Count >= 3)
			{
				QueueScene (new Scene (Helper.Translation.Get ("trains.later", new
				{
					date1 = predictions[1].Date.Localize (),
					date2 = predictions[2].Date.Localize (),
				}), background, LoadPortrait (tv, "Demetrius", 1, 1)));
			}

			// Closing scene: Demetrius signs off.
			QueueScene (new Scene (Helper.Translation.Get ("trains.closing"),
				background, portrait) { SoundAsset = "trains_closing" });

			RunProgram (tv);
		}
	}
}
