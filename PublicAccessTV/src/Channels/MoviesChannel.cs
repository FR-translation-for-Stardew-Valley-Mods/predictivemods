using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.GameData.Movies;
using StardewValley.Objects;
using System.IO;

namespace PublicAccessTV
{
	public class MoviesChannel : Channel
	{
		public MoviesChannel ()
			: base ("movies")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "movies_craneGame.png"));
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
			string hostName =
				Helper.ModRegistry.IsLoaded ("Lemurkat.NPCJuliet")
					? "Juliet"
					: Helper.ModRegistry.IsLoaded ("FlashShifter.StardewValleyExpandedCP")
						? "Claire"
						: Helper.Translation.Get ("movies.host.generic");
			QueueScene (new Scene (Helper.Translation.Get ("movies.opening",
				new { host = hostName }), screenBackground, hostOverlay)
				{ soundCueName = "Cowboy_Secret" });

			// Current movie poster, title and description
			QueueScene (new Scene (Helper.Translation.Get ("movies.current", new
				{
					title = prediction.currentMovie.Title,
					description = prediction.currentMovie.Description,
				}), LoadMoviePoster (tv, prediction.currentMovie))
				{ musicTrack = prediction.currentMovie.Scenes[0].Music });

			// Lobby advertisement. If the crane game is available, it is
			// promoted; otherwise, the concession stand is promoted.
			if (prediction.craneGameAvailable)
			{
				string assetName = Helper.Content.GetActualAssetKey
					(Path.Combine ("assets", "movies_craneGame.png"));
				TemporaryAnimatedSprite craneGame = LoadSprite (tv, assetName,
					new Rectangle (0, 0, 94, 63));
				TemporaryAnimatedSprite craneFlash = LoadSprite (tv, assetName,
					new Rectangle (94, 0, 94, 63), 250f, 2, new Vector2 (),
					true, true);
				QueueScene (new Scene (Helper.Translation.Get ("movies.lobby.craneGame"),
					craneGame, craneFlash) { musicTrack = "crane_game" });
			}
			else
			{
				QueueScene (new Scene (Helper.Translation.Get ("movies.lobby.concession"),
					LoadSprite (tv, "MovieTheater_TileSheet", new Rectangle (2, 3, 84, 56)))
					{ soundAsset = "movies_concession" });
			}

			// Upcoming movie poster, title and description.
			QueueScene (new Scene (Helper.Translation.Get ("movies.next", new
				{
					season = Utility.getSeasonNameFromNumber
						(prediction.firstDateOfNextMovie.SeasonIndex),
					title = prediction.nextMovie.Title,
					description = prediction.nextMovie.Description,
				}), LoadMoviePoster (tv, prediction.nextMovie))
				{ musicTrack = prediction.nextMovie.Scenes[0].Music });

			// Closing scene: the concessionaire signs off.
			QueueScene (new Scene (Helper.Translation.Get ("movies.closing"),
				screenBackground, hostOverlay));

			RunProgram (tv);
		}

		private TemporaryAnimatedSprite LoadMoviePoster (TV tv, MovieData movie)
		{
			return LoadSprite (tv, "LooseSprites\\Movies",
				new Rectangle (15, 128 * movie.SheetIndex, 92, 61));
		}
	}
}
