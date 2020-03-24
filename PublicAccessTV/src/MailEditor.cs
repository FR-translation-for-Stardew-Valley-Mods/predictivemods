using StardewModdingAPI;

namespace PublicAccessTV
{
	internal class MailEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\mail");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;
			string letter = Helper.Translation.Get ("mining.letter.content") +
				"[#]" + Helper.Translation.Get ("mining.letter.title");
			data["kdau.PublicAccessTV.mining"] = letter;
		}
	}
}
