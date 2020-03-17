using StardewModdingAPI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PublicAccessTV
{
	internal class DialogueEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			if (ModEntry.Config.BypassFriendships)
				return false;

			return // TODO: asset.AssetNameEquals ($"Characters\\Dialogue\\{GarbageChannel.DialogueCharacter}") ||
				// TODO: asset.AssetNameEquals ($"Characters\\Dialogue\\{ShoppingChannel.DialogueCharacter}") ||
				// TODO: asset.AssetNameEquals ($"Characters\\Dialogue\\{TailoringChannel.DialogueCharacter}") ||
				asset.AssetNameEquals ($"Characters\\Dialogue\\{TrainsChannel.DialogueCharacter}");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;

			/* TODO:
			if (asset.AssetNameEquals ($"Characters\\Dialogue\\{GarbageChannel.DialogueCharacter}"))
				ApplyDialogue ("garbage", data, GarbageChannel.Dialogue);
			*/

			/* TODO:
			if (asset.AssetNameEquals ($"Characters\\Dialogue\\{ShoppingChannel.DialogueCharacter}"))
				ApplyDialogue ("shopping", data, ShoppingChannel.Dialogue);
			*/

			/* TODO:
			if (asset.AssetNameEquals ($"Characters\\Dialogue\\{TailoringChannel.DialogueCharacter}"))
				ApplyDialogue ("tailoring", data, TailoringChannel.Dialogue);
			*/

			if (asset.AssetNameEquals ($"Characters\\Dialogue\\{TrainsChannel.DialogueCharacter}"))
				ApplyDialogue ("trains", data, TrainsChannel.Dialogue);
		}

		private void ApplyDialogue (string module, IDictionary<string, string> to,
			IDictionary<string, string> from)
		{
			foreach (string key in from.Keys)
			{
				to[key] = Regex.Replace (from[key], @"\{\{([^}]+)\}\}",
					(match) => Helper.Translation.Get ($"{module}.event.{match.Groups[1]}"));
			}
		}
	}
}
