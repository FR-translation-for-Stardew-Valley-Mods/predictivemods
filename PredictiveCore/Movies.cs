using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using System;

namespace PredictiveCore
{
	public struct MoviePrediction
	{
        public WorldDate EffectiveDate;
        public MovieData CurrentMovie;
        public bool CraneGameAvailable;

        public WorldDate FirstDateOfNextMovie;
        public MovieData NextMovie;
	}

	public static class Movies
	{
		// Whether this module should be available for player use.
		public static bool IsAvailable => Game1.player.mailReceived.Contains ("ccMovieTheater");

		// Returns the current and next movie and crane game status as of the
		// given date.
		public static MoviePrediction PredictForDate (WorldDate date)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
			{
				throw new InvalidOperationException ("The Movie Theater is not available.");
			}

			MoviePrediction prediction = new MoviePrediction { EffectiveDate = date };
			prediction.CurrentMovie = MovieTheater.GetMovieForDate (date);

			prediction.FirstDateOfNextMovie = Utilities.GetNextSeasonStart (date);
			prediction.NextMovie = MovieTheater.GetMovieForDate (prediction.FirstDateOfNextMovie);

			// Logic from StardewValley.Locations.MovieTheater.addRandomNPCs()
			// as implemented in Stardew Predictor by MouseyPounds.
			Random rng = new Random ((int) Game1.uniqueIDForThisGame + date.TotalDays);
			prediction.CraneGameAvailable = rng.NextDouble () >= 0.25;

			return prediction;
		}

		internal static void Initialize ()
		{
			Utilities.Helper.ConsoleCommands.Add ("predict_movies",
				"Predicts the current and next movie and Crane Game status on a given date, or today by default.\n\nUsage: predict_movies [<year> <season> <day>]\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				ConsoleCommand);
		}

		private static void ConsoleCommand (string _command, string[] args)
		{
			try
			{
				WorldDate date = (args.Length == 0)
					? Game1.Date
					: Utilities.ArgsToWorldDate (args);
				MoviePrediction prediction = PredictForDate (date);
				Utilities.Monitor.Log ($"On {prediction.EffectiveDate.Localize ()}, the movie showing will be '{prediction.CurrentMovie.Title}'.\n  {prediction.CurrentMovie.Description}\nThe Crane Game {(prediction.CraneGameAvailable ? "WILL" : "will NOT")} be available.\n\nThe next movie, '{prediction.NextMovie.Title}', will begin showing on {prediction.FirstDateOfNextMovie.Localize ()}.\n  {prediction.NextMovie.Description}",
					LogLevel.Info);
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Alert);
			}
		}
	}
}
