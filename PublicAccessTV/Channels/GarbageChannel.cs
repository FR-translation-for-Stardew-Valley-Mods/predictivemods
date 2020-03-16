using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.IO;
using SObject = StardewValley.Object;

namespace PublicAccessTV
{
	public class GarbageChannel : Channel
	{
		public GarbageChannel ()
			: base ("garbage")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "garbage_backgrounds.png"));
		}

		internal override bool IsAvailable =>
			base.IsAvailable && Garbage.IsAvailable &&
			(ModEntry.Config.BypassFriendships ||
				Game1.player.getFriendshipHeartLevelForNPC ("Linus") >= 4);

		internal override void Show (TV tv)
		{
			WorldDate today = Utilities.Now ();
			List<GarbagePrediction> predictions = Garbage.ListLootForDate (today);

			int seasonIndex = today.SeasonIndex;
			TemporaryAnimatedSprite background = LoadBackground (tv, 0, seasonIndex);
			TemporaryAnimatedSprite portrait = LoadPortrait (tv, "Linus");

			// Opening scene: Linus greets the viewer.
			QueueScene (new Scene (Helper.Translation.Get ("garbage.opening", new
				{
					playerName = Game1.player.Name,
				}), background, portrait) { MusicTrack = "echos" });

			// Linus sadly notes that the cans are empty today.
			if (predictions.Count < 1)
			{
				QueueScene (new Scene (Helper.Translation.Get ("garbage.none"),
					background, LoadPortrait (tv, "Linus", 0, 1))
					{ MusicTrack = "echos" });
			}

			// Linus reports on the content of each non-empty can.
			foreach (GarbagePrediction prediction in predictions)
			{
				string type;
				TemporaryAnimatedSprite reactionPortrait;
				if (prediction.Loot is Hat)
				{
					type = "garbageHat";
					reactionPortrait = LoadPortrait (tv, "Linus", 1, 1);
				}
				else if (prediction.Loot is SObject o && o.Flipped)
				{
					type = "special";
					reactionPortrait = LoadPortrait (tv, "Linus", 1, 0);
				}
				else
				{
					type = "generic";
					reactionPortrait = portrait;
				}

				QueueScene (new Scene
					(Helper.Translation.Get ($"garbage.can.{prediction.Can}") + "^...^" +
						Helper.Translation.Get ($"garbage.item.{type}", new
						{
							itemName = (prediction.Loot.ParentSheetIndex == 217)
								? Helper.Translation.Get ("garbage.dishOfTheDay")
								: prediction.Loot.DisplayName,
						}),
					LoadBackground (tv, (int) prediction.Can + 1, seasonIndex),
					reactionPortrait)
					{ MusicTrack = "echos" });
			}

			// Closing scene: Linus signs off.
			bool progress = Garbage.IsProgressDependent;
			QueueScene (new Scene
				(Helper.Translation.Get ($"garbage.closing.{(progress? "progress" : "standard")}"),
				background, portrait) { MusicTrack = "echos" });

			RunProgram (tv);
		}
	}
}
