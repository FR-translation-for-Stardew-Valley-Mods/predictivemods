using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ScryingOrb
{
	internal class CursorEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.DataType == typeof (Texture2D) &&
				asset.AssetNameEquals ("LooseSprites\\Cursors") &&
				ModEntry.OrbsIlluminated > 0;
		}

		public void Edit<_T> (IAssetData asset)
		{
			IAssetDataForImage editor = asset.AsImage ();
			Rectangle bounds = new Rectangle (0, 0, 16, 16);
			Texture2D source = Helper.Content.Load<Texture2D> ("assets/cursor.png");
			editor.PatchImage (source, bounds, bounds);
		}

		internal void Invalidate ()
		{
			Helper.Content.InvalidateCache ((asset) =>
				asset.AssetNameEquals ("LooseSprites\\Cursors"));
		}
	}
}
