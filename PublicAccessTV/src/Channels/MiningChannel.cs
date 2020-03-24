﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PublicAccessTV
{
	public class MiningChannel : Channel
	{
		public static readonly List<MineFloorType> GilTypes =
			new List<MineFloorType>
		{
			MineFloorType.Mushroom,
			MineFloorType.Treasure,
			MineFloorType.PepperRex,
		};

		public MiningChannel ()
			: base ("mining")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "mining_backgrounds.png"));
		}

		internal override bool IsAvailable =>
			base.IsAvailable && Mining.IsAvailable &&
			(Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.mining") ||
				Game1.player.mailbox.Contains ("kdau.PublicAccessTV.mining"));

		internal override void Update ()
		{
			if (base.IsAvailable && Mining.IsAvailable &&
				!Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.mining") &&
				!Game1.player.mailbox.Contains ("kdau.PublicAccessTV.mining") &&
				Game1.player.mailReceived.Contains ("guildMember") &&
				(ModEntry.Config.BypassFriendships ||
					!Helper.ModRegistry.IsLoaded ("FlashShifter.MarlonSVE") ||
					Game1.player.getFriendshipHeartLevelForNPC ("MarlonFay") >= 2))
			{
				Game1.player.mailbox.Add ("kdau.PublicAccessTV.mining");
			}

			base.Update ();
		}

		internal override void Reset ()
		{
			Game1.player.mailReceived.Remove ("kdau.PublicAccessTV.mining");
			Game1.player.mailbox.Remove ("kdau.PublicAccessTV.mining");
		}

		internal override void Show (TV tv)
		{
			WorldDate today = Utilities.Now ();
			List<MiningPrediction> predictions = Mining.ListFloorsForDate (today);

			TemporaryAnimatedSprite background = LoadBackground (tv, 0);
			TemporaryAnimatedSprite marlon = LoadPortrait (tv, "Marlon");
			TemporaryAnimatedSprite gil = LoadPortrait (tv, "Gil");

			// Opening scene: Marlon greets the viewer.
			QueueScene (new Scene (Helper.Translation.Get ((predictions.Count == 0)
				? "mining.opening.none" : "mining.opening"),
				background, marlon) { musicTrack = "MarlonsTheme" });

			// Marlon or Gil reports on each type of special floor.
			string joiner = CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ";
			foreach (MineFloorType type in predictions
				.Select ((p) => p.type).Distinct ().ToList ())
			{
				List<int> floors = predictions
					.Where ((p) => p.type == type)
					.Select ((p) => p.floor)
					.ToList ();
				string floorsText;
				if (floors.Count == 1)
				{
					floorsText = Helper.Translation.Get ("mining.floor",
						new { num = floors[0] });
				}
				else
				{
					int lastNum = floors[floors.Count - 1];
					floors.RemoveAt (floors.Count - 1);
					floorsText = Helper.Translation.Get ("mining.floors",
						new { nums = string.Join (joiner, floors), lastNum = lastNum });
				}

				QueueScene (new Scene (Helper.Translation.Get ($"mining.prediction.{type}",
						new { floors = floorsText, }),
					LoadBackground (tv, (int) type + 1),
					GilTypes.Contains (type) ? gil : marlon)
					{ musicTrack = "MarlonsTheme" });
			}

			// Closing scene: Marlon signs off.
			bool progress = Mining.IsProgressDependent;
			QueueScene (new Scene
				(Helper.Translation.Get ($"mining.closing.{(progress? "progress" : "standard")}"),
				background, marlon) { musicTrack = "MarlonsTheme" });

			RunProgram (tv);
		}
	}
}