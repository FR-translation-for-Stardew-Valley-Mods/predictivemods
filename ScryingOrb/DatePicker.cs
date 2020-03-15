using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScryingOrb
{
	internal class DatePicker : IClickableMenu
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;
		
		private readonly WorldDate InitialDate;
		private WorldDate Date;

		private readonly string PromptMessage;
		private readonly Action<WorldDate> OnConfirm;

		private readonly Texture2D CalendarTile;
		private readonly Texture2D DayButtonTiles;
		
		private string HoverText;
		private int HoverButton = -999;
		private int SelectedDay = -1;
		private List<TemporaryAnimatedSprite> DaySparkles =
			new List<TemporaryAnimatedSprite> ();
		private List<int> SeasonSpriteHits = new List<int> { 0, 0, 0, 0 };
		private List<WeatherDebris> SeasonDebris = new List<WeatherDebris> ();

		private ClickableComponent PromptLabel;
		private ClickableComponent DateLabel;
		private List<ClickableComponent> WeekLabels;
		private ClickableTextureComponent Calendar;
		private List<ClickableTextureComponent> DayButtons;
		private List<ClickableTextureComponent> SeasonSprites;
		private ClickableTextureComponent PrevButton;
		private ClickableTextureComponent NextButton;
		private ClickableTextureComponent ScryButton;
		private List<ClickableTextureComponent> OtherButtons;

		private static int CalendarSize = 500;

		private static int Width => CalendarSize +
			borderWidth * 2 + spaceToClearSideBorder * 4;
		private static int Height => Game1.smallFont.LineSpacing +
			Game1.dialogueFont.LineSpacing + CalendarSize + Game1.tileSize +
			borderWidth * 2 + spaceToClearTopBorder + spaceToClearSideBorder * 6;

		private static int X => (Game1.viewport.Width - Width) / 2;
		private static int Y => (Game1.viewport.Height - Height) / 2;

		private struct SeasonDataT
		{
			public readonly Color MainColor;
			public readonly Rectangle SpriteBounds;
			public readonly string SpriteAsset;
			public readonly Rectangle SpriteSource;
			public readonly int SpriteTextColor;

			public SeasonDataT (Color mainColor, Rectangle spriteBounds,
				string spriteAsset, Rectangle spriteSource, int spriteTextColor)
			{
				MainColor = mainColor;
				SpriteBounds = spriteBounds;
				SpriteAsset = spriteAsset;
				SpriteSource = spriteSource;
				SpriteTextColor = spriteTextColor;
			}
		}

		private static readonly List<SeasonDataT> SeasonData = new List<SeasonDataT>
		{
			new SeasonDataT (new Color ( 54, 179,  67), new Rectangle ( 191, -242, 48, 54), "TileSheets\\crops", new Rectangle (112, 522, 48, 54), SpriteText.color_Green),
			new SeasonDataT (new Color (143,  63, 204), new Rectangle ( 191,  194, 48, 54), "TileSheets\\crops", new Rectangle (160, 518, 48, 54), SpriteText.color_Purple),
			new SeasonDataT (new Color (212,  50,   0), new Rectangle (-239,  194, 48, 54), "TileSheets\\crops", new Rectangle (208, 518, 48, 54), SpriteText.color_Red),
			new SeasonDataT (new Color ( 12, 130, 181), new Rectangle (-239, -239, 48, 48), "Maps\\winter_town", new Rectangle (288, 384, 48, 48), SpriteText.color_Blue),
		};

		public DatePicker (WorldDate initialDate, string promptMessage,
			Action<WorldDate> onConfirm)
			: base (X, Y, Width, Height)
		{
			InitialDate = initialDate;
			Date = initialDate;

			int initialYearStart = (InitialDate.Year - 1) * 112;
			SelectedDay = InitialDate.TotalDays - initialYearStart;

			PromptMessage = promptMessage;
			OnConfirm = onConfirm;

			CalendarTile = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "calendar.png"));
			DayButtonTiles = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "dayButton.png"));

			ArrangeInterface ();
		}
		
		public override void gameWindowSizeChanged (Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged (oldBounds, newBounds);
			xPositionOnScreen = X;
			yPositionOnScreen = Y;
			ArrangeInterface ();
		}

		private void ArrangeInterface ()
		{
			int xOff = xPositionOnScreen + borderWidth + spaceToClearSideBorder * 2;
			int yOff = yPositionOnScreen + borderWidth + spaceToClearTopBorder;

			PromptLabel = new ClickableComponent (
				new Rectangle (xPositionOnScreen, yOff, width, Game1.smallFont.LineSpacing),
				"PromptLabel");
			yOff += Game1.smallFont.LineSpacing + spaceToClearSideBorder;

			DateLabel = new ClickableComponent (
				new Rectangle (xOff, yOff, CalendarSize, Game1.dialogueFont.LineSpacing),
				"DateLabel");
			yOff += Game1.dialogueFont.LineSpacing + spaceToClearSideBorder * 2;

			Calendar = new ClickableTextureComponent ("Calendar",
				new Rectangle (xOff, yOff, CalendarSize, CalendarSize),
				null, null, CalendarTile, new Rectangle (), 1f, true);
			int xCenter = xOff + CalendarSize / 2;
			int yCenter = yOff + CalendarSize / 2;
			yOff += CalendarSize + spaceToClearSideBorder * 2;

			WeekLabels = new List<ClickableComponent> ();
			double weekRadius = 164.5;
			for (int i = 0; i < 16; ++i)
			{
				double angle = 2 * Math.PI * (i + 0.5) / 16.0;
				int x = (int) (xCenter + weekRadius * Math.Sin (angle)) +
					((i % 4 == 0) ? 0 : 4);
				int y = (int) (yCenter - weekRadius * Math.Cos (angle)) - 24;

				WeekLabels.Add (new ClickableComponent (new Rectangle (x, y, 0, 0),
					$"Week{i}"));
			}

			DayButtons = new List<ClickableTextureComponent> ();
			double dayRadius = 220.0;
			for (int i = 0; i < 112; ++i)
			{
				WorldDate date = DayToWorldDate (i);

				double angle = 2 * Math.PI * (i + 0.5) / 112.0;
				int x = (int) (xCenter + dayRadius * Math.Sin (angle)) - 6;
				int y = (int) (yCenter - dayRadius * Math.Cos (angle)) - 26;

				DayButtons.Add (new ClickableTextureComponent ($"Day{i}",
					new Rectangle (x, y, 12, 52), null, date.Localize (),
					DayButtonTiles, new Rectangle (36 * (i / 28), 0, 12, 52),
					1f));
			}

			SeasonSprites = new List<ClickableTextureComponent> ();
			for (int i = 0; i < 4; ++i)
			{
				Texture2D texture = Helper.Content.Load<Texture2D>
					(SeasonData[i].SpriteAsset, ContentSource.GameContent);
				Rectangle sb = SeasonData[i].SpriteBounds;
				Rectangle bounds = new Rectangle (sb.X + xCenter, sb.Y + yCenter,
					sb.Width, sb.Height);
				SeasonSprites.Add (new ClickableTextureComponent ($"SeasonSprite{i}",
					bounds, null, null, texture, SeasonData[i].SpriteSource, 1f));
			}

			xOff -= spaceToClearSideBorder;

			PrevButton = new ClickableTextureComponent ("PrevButton",
				new Rectangle (xOff, yOff, Game1.tileSize, Game1.tileSize), null,
				Helper.Translation.Get ("datePicker.prevLabel"), Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 44),
				1f);

			NextButton = new ClickableTextureComponent ("NextButton",
				new Rectangle (xOff + Game1.tileSize + spaceToClearSideBorder, yOff,
					Game1.tileSize, Game1.tileSize), null,
				Helper.Translation.Get ("datePicker.nextLabel"), Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 33),
				1f);

			ScryButton = new ClickableTextureComponent ("ScryButton",
				new Rectangle (xOff + CalendarSize + spaceToClearSideBorder * 2
					- Game1.tileSize, yOff, Game1.tileSize, Game1.tileSize), null,
				Helper.Translation.Get ("datePicker.scryLabel"), Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet (Game1.mouseCursors, 46),
				1f);

			OtherButtons = new List<ClickableTextureComponent>
				{ PrevButton, NextButton, ScryButton };
		}

		public override void receiveLeftClick (int x, int y, bool playSound = true)
		{
			base.receiveLeftClick (x, y, playSound);

			int day = GetDayAtPoint (x, y);
			if (day > -1)
			{
				if (playSound) Game1.playSound ("newArtifact");
				SelectDay (day);
				return;
			}

			for (int i = 0; i < SeasonSprites.Count; ++i)
			{
				if (SeasonSprites[i].containsPoint (x, y))
				{
					if (playSound) Game1.playSound ("leafrustle");
					HitSeasonSprite (i);
					return;
				}
			}

			if (PrevButton.containsPoint (x, y))
			{
				PrevButton.scale = 1f;
				if (playSound) Game1.playSound ("newArtifact");
				SelectPrev ();
				return;
			}

			if (NextButton.containsPoint (x, y))
			{
				NextButton.scale = 1f;
				if (playSound) Game1.playSound ("newArtifact");
				SelectNext ();
				return;
			}

			if (ScryButton.containsPoint (x, y))
			{
				ScryButton.scale = 1f;
				if (playSound) Game1.playSound ("select");
				Confirm ();
				return;
			}
		}

		public override void receiveKeyPress (Keys key)
		{
			base.receiveKeyPress (key);

			switch (key)
			{
			case Keys.Left:
				Game1.playSound ("newArtifact");
				SelectPrev ();
				break;
			case Keys.Right:
				Game1.playSound ("newArtifact");
				SelectNext ();
				break;
			case Keys.Enter:
				Game1.playSound ("select");
				Confirm ();
				break;
			}
		}

		private void SelectDay (int day)
		{
			SelectedDay = day;
			Date = DayToWorldDate (day);
			Rectangle bounds = new Rectangle (
				DayButtons[day].bounds.X - 30,
				DayButtons[day].bounds.Y - 10, 20, 20);
			DaySparkles = Utility.sparkleWithinArea (bounds, 2,
				SeasonData[day / 28].MainColor, 50);
		}

		private WorldDate DayToWorldDate (int day)
		{
			int initialYearStart = (InitialDate.Year - 1) * 112;
			int initialDay = InitialDate.TotalDays - initialYearStart;
			int offset = (day < initialDay) ? 112 : 0;
			return Utilities.TotalDaysToWorldDate (initialYearStart + day + offset);
		}

		private void SelectPrev ()
		{
			SelectDay ((SelectedDay == 0) ? 111 : SelectedDay - 1);
		}

		private void SelectNext ()
		{
			SelectDay ((SelectedDay == 111) ? 0 : SelectedDay + 1);
		}

		private void Confirm ()
		{
			Game1.exitActiveMenu ();
			OnConfirm (Date);
		}

		private void HitSeasonSprite (int seasonIndex)
		{
			if (++SeasonSpriteHits[seasonIndex] < 4)
				return;
			SeasonSpriteHits[seasonIndex] = 0;

			Random rng = new Random ();
			Rectangle spriteBounds = SeasonSprites[seasonIndex].bounds;

			SeasonDebris.Clear ();
			int debrisCount = ((seasonIndex == 3) ? 3 : 1) * (5 + rng.Next (0, 5));
			for (int i = 0; i < debrisCount; ++i)
			{
				Vector2 position = new Vector2 (spriteBounds.X + rng.Next (0, 48),
					spriteBounds.Y + rng.Next (0, 48));
				SeasonDebris.Add (new WeatherDebris (position, seasonIndex,
					rng.Next (15) / 500f,
					rng.Next (-10, 10) / 10f + ((seasonIndex > 1) ? 1f : -1f),
					rng.Next (-10, 10) / 10f + ((seasonIndex % 3 == 0) ? 0.75f : -2f)));
			}
		}

		public override void performHoverAction (int x, int y)
		{
			base.performHoverAction (x, y);
			HoverText = null;
			int oldHoverButton = HoverButton;

			int day = GetDayAtPoint (x, y);
			if (day > -1)
			{
				if (HoverButton != day)
				{
					HoverButton = day;
					Game1.playSound ("Cowboy_gunshot");
				}
				HoverText = DayButtons[day].hoverText;
			}
			else
			{
				HoverButton = -999;
			}

			for (int i = 0; i < OtherButtons.Count; ++i)
			{
				ClickableTextureComponent button = OtherButtons[i];
				if (button.containsPoint (x, y))
				{
					HoverButton = -1 - i;
					if (oldHoverButton != HoverButton)
						Game1.playSound ("Cowboy_Footstep");
					HoverText = button.hoverText;
				}
				button.scale = button.containsPoint (x, y)
					? Math.Min (button.scale + 0.02f, button.baseScale + 0.1f)
					: Math.Max (button.scale - 0.02f, button.baseScale);
			}
		}

		private int GetDayAtPoint (int x, int y)
		{
			x -= Calendar.bounds.Center.X;
			y -= Calendar.bounds.Center.Y;
			double radius = Math.Sqrt (Math.Pow (x, 2) + Math.Pow (y, 2));
			if (radius < 192.0 || radius > 248.0) return -1;
			double pct = Math.Atan2 (y, x) / (2 * Math.PI);
			return (int) Math.Floor (140.0 + 112.0 * pct) % 112;
		}

		public override void update (GameTime time)
		{
			for (int i = DaySparkles.Count - 1; i >= 0; --i)
			{
				if (DaySparkles[i].update (time))
					DaySparkles.RemoveAt (i);
			}

			foreach (WeatherDebris debris in SeasonDebris)
				debris.update ();
		}

		public override void draw (SpriteBatch b)
		{
			// dialog background
			Game1.drawDialogueBox (xPositionOnScreen, yPositionOnScreen, width,
				height, false, true);

			// PromptLabel
			float promptWidth = Game1.smallFont.MeasureString (PromptMessage).X;
			float promptOffset = (PromptLabel.bounds.Width - promptWidth) / 2;
			Utility.drawTextWithShadow (b, PromptMessage, Game1.smallFont,
				new Vector2 (PromptLabel.bounds.X + promptOffset, PromptLabel.bounds.Y),
				Game1.textColor);

			// DateLabel
			string dateText = Date.Localize ();
			float dateWidth = Game1.dialogueFont.MeasureString (dateText).X;
			float dateOffset = (DateLabel.bounds.Width - dateWidth) / 2;
			Utility.drawTextWithShadow (b, dateText, Game1.dialogueFont,
				new Vector2 (DateLabel.bounds.X + dateOffset, DateLabel.bounds.Y),
				Game1.textColor);

			// Calendar
			Calendar.draw (b);

			// WeekLabels
			for (int i = 0; i < WeekLabels.Count; ++i)
			{
				ClickableComponent label = WeekLabels[i];
				string text = (i % 4 + 1).ToString ();
				SpriteText.drawStringHorizontallyCenteredAt (b, text,
					label.bounds.X, label.bounds.Y, junimoText: true);
			}

			// DayButtons
			for (int i = 0; i < DayButtons.Count; ++i)
			{
				ClickableTextureComponent button = DayButtons[i];
				Vector2 position = new Vector2 ((float) button.bounds.X +
					(float) (button.sourceRect.Width / 2) * button.baseScale,
					(float) button.bounds.Y + (float) (button.sourceRect.Height / 2)
					* button.baseScale);
				Rectangle sourceRect = new Rectangle (button.sourceRect.X +
					((i == SelectedDay) ? 24 : (i == HoverButton) ? 12 : 0),
					button.sourceRect.Y, button.sourceRect.Width,
					button.sourceRect.Height);
				Vector2 origin = new Vector2 (button.sourceRect.Width / 2,
					button.sourceRect.Height / 2);
				double angle = 2 * Math.PI * (i + 0.5) / 112.0;
				b.Draw (button.texture, position, sourceRect, Color.White,
					(float) angle, origin, 1f, SpriteEffects.None,
					0.86f + (float) button.bounds.Y / 20000f);
			}

			// SeasonSprites
			foreach (ClickableTextureComponent sprite in SeasonSprites)
				sprite.draw (b);

			// PrevButton, NextButton, ScryButton
			foreach (ClickableTextureComponent button in OtherButtons)
				button.draw (b);

			// SeasonDebris
			foreach (WeatherDebris debris in SeasonDebris)
				debris.draw (b);

			// DaySparkles
			foreach (TemporaryAnimatedSprite sparkle in DaySparkles)
				sparkle.draw (b, true);

			// hover text
			if (HoverText != null)
				drawHoverText (b, HoverText, Game1.smallFont);

			// mouse cursor
			if (!Game1.options.hardwareCursor)
				drawMouse (b);
		}
	}
}