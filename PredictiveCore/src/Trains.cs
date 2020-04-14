﻿using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PredictiveCore
{
	public static class Trains
	{
		public struct Prediction
		{
			public WorldDate date;
			public int time;
		}

		// Whether this module should be available for player use.
		public static bool IsAvailable =>
			Utilities.Now ().TotalDays >= 30 &&
			(Utilities.Config.InaccuratePredictions ||
				!Utilities.Helper.ModRegistry.IsLoaded ("AairTheGreat.BetterTrainLoot"));

		// Lists the next several trains to arrive on or after the given date,
		// up to the given limit.
		public static List<Prediction> ListNextTrainsFromDate
			(WorldDate fromDate, uint limit)
		{
			Utilities.CheckWorldReady ();
			if (!IsAvailable)
				throw new InvalidOperationException ("The Railroad is not available.");

			// Logic from StardewValley.Locations.Railroad.DayUpdate()
			// as implemented in Stardew Predictor by MouseyPounds.

			List<Prediction> predictions = new List<Prediction> ();

			for (int days = Math.Max (fromDate.TotalDays, 30);
				predictions.Count < limit &&
					days < fromDate.TotalDays + Utilities.MaxHorizon;
				++days)
			{
				Random rng = new Random (((int) Game1.uniqueIDForThisGame / 2) +
					days + 1);
				if (!(rng.NextDouble () < 0.2))
					continue;

				int time = rng.Next (900, 1800);
				time -= time % 10;
				if (time % 100 >= 60)
					continue;

				WorldDate date = Utilities.TotalDaysToWorldDate (days);
				predictions.Add (new Prediction { date = date, time = time });
			}

			return predictions;
		}

		internal static void Initialize (bool addConsoleCommands)
		{
			if (!addConsoleCommands)
				return;
			Utilities.Helper.ConsoleCommands.Add ("predict_trains",
				"Predicts the next several trains to arrive on or after a given date, or today by default.\n\nUsage: predict_trains [<limit> [<year> <season> <day>]]\n- limit: number of trains to predict (default 20)\n- year: the target year (a number starting from 1).\n- season: the target season (one of 'spring', 'summer', 'fall', 'winter').\n- day: the target day (a number from 1 to 28).",
				(_command, args) => ConsoleCommand (args.ToList ()));
		}

		private static void ConsoleCommand (List<string> args)
		{
			try
			{
				uint limit = 20;
				if (args.Count > 0)
				{
					if (!uint.TryParse (args[0], out limit) || limit < 1)
						throw new ArgumentException ($"Invalid limit '{args[0]}', must be a number 1 or higher.");
					args.RemoveAt (0);
				}
				WorldDate date = Utilities.ArgsToWorldDate (args);

				List<Prediction> predictions = ListNextTrainsFromDate (date, limit);
				Utilities.Monitor.Log ($"Next {limit} train(s) arriving on or after {date}:",
					LogLevel.Info);
				foreach (Prediction prediction in predictions)
				{
					Utilities.Monitor.Log ($"- {prediction.date} at {Game1.getTimeOfDayString (prediction.time)}",
						LogLevel.Info);
				}
			}
			catch (Exception e)
			{
				Utilities.Monitor.Log (e.Message, LogLevel.Error);
			}
		}
	}
}
