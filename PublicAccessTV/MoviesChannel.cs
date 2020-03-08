using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.GameData.Movies;
using StardewValley.Objects;

namespace PublicAccessTV
{
	public class MoviesChannel : Channel
	{
		public MoviesChannel ()
			: base ("movies")
		{
			Helper.Content.Load<Texture2D> ("assets/movies/cranegame.png");
		}

		internal override bool IsAvailable => base.IsAvailable && Movies.IsAvailable;

		internal override void Show (TV tv)
		{
			MoviePrediction prediction = Movies.PredictForDate (Game1.Date);

			TemporaryAnimatedSprite claireBackground = LoadSprite (tv,
				"MovieTheaterScreen_TileSheet", new Rectangle (31, 0, 162, 108));
			TemporaryAnimatedSprite claireOverlay = LoadSprite (tv,
				"MovieTheater_TileSheet", new Rectangle (240, 160, 16, 26),
				9999f, 1, new Vector2 (18f, 2f), true);

			// Opening scene: Claire greets the viewer.
			QueueScene (Helper.Translation.Get ("movies.opening"),
				claireBackground, claireOverlay);

			// Current movie poster, title and description
			QueueScene (Helper.Translation.Get ("movies.current", new
			{
				title = prediction.CurrentMovie.Title,
				description = prediction.CurrentMovie.Description,
			}), LoadMoviePoster (tv, prediction.CurrentMovie));

			// Lobby advertisement. If the crane game is available, it is
			// shown/promoted; otherwise, the concession stand is shown/promoted.
			string lobbyMessage;
			TemporaryAnimatedSprite lobbyBackground;
			TemporaryAnimatedSprite lobbyOverlay;
			if (prediction.CraneGameAvailable)
			{
				lobbyMessage = Helper.Translation.Get ("movies.lobby.cranegame");
				string assetName = Helper.Content.GetActualAssetKey ("assets/movies/cranegame.png");
				lobbyBackground = LoadSprite (tv, assetName,
					new Rectangle (0, 0, 94, 63));
				lobbyOverlay = LoadSprite (tv, assetName,
					new Rectangle (94, 0, 94, 63), 250f, 2, new Vector2 (),
					true, true);
			}
			else
			{
				lobbyMessage = Helper.Translation.Get ("movies.lobby.concession");
				lobbyBackground = LoadSprite (tv, "MovieTheater_TileSheet",
					new Rectangle (2, 3, 84, 56));
				lobbyOverlay = null;
			}
			QueueScene (lobbyMessage, lobbyBackground, lobbyOverlay);

			// Upcoming movie poster, title and description.
			QueueScene (Helper.Translation.Get ("movies.next", new
			{
				date = prediction.FirstDateOfNextMovie.Localize (),
				title = prediction.NextMovie.Title,
				description = prediction.NextMovie.Description,
			}), LoadMoviePoster (tv, prediction.NextMovie));

			// Closing scene: Claire signs off.
			QueueScene (Helper.Translation.Get ("movies.closing"),
				claireBackground, claireOverlay);

			RunProgram (tv);
		}

		private TemporaryAnimatedSprite LoadMoviePoster (TV tv, MovieData movie)
		{
			return LoadSprite (tv, "LooseSprites\\Movies",
				new Rectangle (16, 128 * movie.SheetIndex, 90, 61));
		}
	}
}
