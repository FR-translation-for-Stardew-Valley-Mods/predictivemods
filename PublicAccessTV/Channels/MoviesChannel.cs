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
			Helper.Content.Load<Texture2D> ("assets/movies_craneGame.png");
		}

		internal override bool IsAvailable =>
			base.IsAvailable && Movies.IsAvailable;

		internal override void Show (TV tv)
		{
			MoviePrediction prediction = Movies.PredictForDate (Utilities.Now ());

			TemporaryAnimatedSprite screenBackground = LoadSprite (tv,
				"MovieTheaterScreen_TileSheet", new Rectangle (31, 0, 162, 108));
			TemporaryAnimatedSprite hostOverlay = LoadSprite (tv,
				"MovieTheater_TileSheet", new Rectangle (240, 160, 16, 26),
				positionOffset: new Vector2 (18f, 2f), overlay: true);

			// Opening scene: the concessionaire greets the viewer.
			bool sve = Helper.ModRegistry.IsLoaded ("FlashShifter.StardewValleyExpandedCP");
			QueueScene (Helper.Translation.Get ("movies.opening", new
			{
				host = Helper.Translation.Get ($"movies.host.{(sve ? "sve" : "base")}"),
			}), screenBackground, hostOverlay);

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
				lobbyMessage = Helper.Translation.Get ("movies.lobby.craneGame");
				string assetName = Helper.Content.GetActualAssetKey ("assets/movies_craneGame.png");
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
				season = Helper.Translation.Get ($"season.{prediction.FirstDateOfNextMovie.Season}"),
				title = prediction.NextMovie.Title,
				description = prediction.NextMovie.Description,
			}), LoadMoviePoster (tv, prediction.NextMovie));

			// Closing scene: the concessionaire signs off.
			QueueScene (Helper.Translation.Get ("movies.closing"),
				screenBackground, hostOverlay);

			RunProgram (tv);
		}

		private TemporaryAnimatedSprite LoadMoviePoster (TV tv, MovieData movie)
		{
			return LoadSprite (tv, "LooseSprites\\Movies",
				new Rectangle (15, 128 * movie.SheetIndex, 92, 61));
		}
	}
}
