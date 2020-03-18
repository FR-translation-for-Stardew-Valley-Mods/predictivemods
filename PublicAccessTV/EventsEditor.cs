﻿using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PublicAccessTV
{
	internal class EventsEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			if (ModEntry.Config.BypassFriendships)
				return false;

			return asset.AssetNameEquals ($"Data\\Events\\{GarbageChannel.EventMap}") ||
				// TODO: asset.AssetNameEquals ($"Data\\Events\\{ShoppingChannel.EventMap}") ||
				// TODO: asset.AssetNameEquals ($"Data\\Events\\{TailoringChannel.EventMap}") ||
				asset.AssetNameEquals ($"Data\\Events\\{TrainsChannel.EventMap}");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;

			if (asset.AssetNameEquals ($"Data\\Events\\{GarbageChannel.EventMap}"))
				ApplyEvents ("garbage", data, GarbageChannel.Events);

			/* TODO:
			if (asset.AssetNameEquals ($"Data\\Events\\{ShoppingChannel.EventMap}"))
				ApplyEvents ("shopping", data, ShoppingChannel.Events);
			*/

			/* TODO:
			if (asset.AssetNameEquals ($"Data\\Events\\{TailoringChannel.EventMap}"))
				ApplyEvents ("tailoring", data, TailoringChannel.Events);
			*/

			if (asset.AssetNameEquals ($"Data\\Events\\{TrainsChannel.EventMap}"))
				ApplyEvents ("trains", data, TrainsChannel.Events);
		}

		private void ApplyEvents (string module, IDictionary<string, string> to,
			IDictionary<string, string> from)
		{
			foreach (string key in from.Keys.ToList ())
			{
				to[key] = from[key] = Regex.Replace (from[key], @"\{\{([^}]+)\}\}",
					(match) => Helper.Translation.Get ($"{module}.event.{match.Groups[1]}"));
			}
		}
	}
}
