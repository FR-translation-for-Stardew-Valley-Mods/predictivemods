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
				{ SoundCueName = "Cowboy_Secret" });

			// Current movie poster, title and description
			QueueScene (new Scene (Helper.Translation.Get ("movies.current", new
				{
					title = prediction.CurrentMovie.Title,
					description = prediction.CurrentMovie.Description,
				}), LoadMoviePoster (tv, prediction.CurrentMovie))
				{ MusicTrack = prediction.CurrentMovie.Scenes[0].Music });

			// Lobby advertisement. If the crane game is available, it is
			// promoted; otherwise, the concession stand is promoted.
			if (prediction.CraneGameAvailable)
			{
				string assetName = Helper.Content.GetActualAssetKey
					(Path.Combine ("assets", "movies_craneGame.png"));
				TemporaryAnimatedSprite craneGame = LoadSprite (tv, assetName,
					new Rectangle (0, 0, 94, 63));
				TemporaryAnimatedSprite craneFlash = LoadSprite (tv, assetName,
					new Rectangle (94, 0, 94, 63), 250f, 2, new Vector2 (),
					true, true);
				QueueScene (new Scene (Helper.Translation.Get ("movies.lobby.craneGame"),
					craneGame, craneFlash) { MusicTrack = "crane_game" });
			}
			else
			{
				QueueScene (new Scene (Helper.Translation.Get ("movies.lobby.concession"),
					LoadSprite (tv, "MovieTheater_TileSheet", new Rectangle (2, 3, 84, 56)))
					{ SoundAsset = "movies_concession" });
			}

			// Upcoming movie poster, title and description.
			QueueScene (new Scene (Helper.Translation.Get ("movies.next", new
				{
					season = Utility.getSeasonNameFromNumber
						(prediction.FirstDateOfNextMovie.SeasonIndex),
					title = prediction.NextMovie.Title,
					description = prediction.NextMovie.Description,
				}), LoadMoviePoster (tv, prediction.NextMovie))
				{ MusicTrack = prediction.NextMovie.Scenes[0].Music });

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
