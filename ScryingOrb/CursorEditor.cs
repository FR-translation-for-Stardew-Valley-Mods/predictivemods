using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.IO;

namespace ScryingOrb
{
	internal class CursorEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;

		protected Texture2D cursor;
		protected bool pendingInvalidate;
		protected int lastInvalidate;

		public CursorEditor()
		{
			cursor = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "cursor.png"));
		}

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.DataType == typeof (Texture2D) &&
				asset.AssetNameEquals ("LooseSprites\\Cursors") &&
				(ModEntry.OrbHovered || ModEntry.OrbsIlluminated > 0);
		}

		public void Edit<_T> (IAssetData asset)
		{
			IAssetDataForImage editor = asset.AsImage ();
			Rectangle bounds = new Rectangle (0, 0, 16, 16);
			editor.PatchImage (cursor, bounds, bounds);
		}

		internal void Invalidate ()
		{
			if (pendingInvalidate)
			{
				return;
			}
			pendingInvalidate = true;
			DelayedAction.functionAfterDelay (() =>
			{
				if (pendingInvalidate)
				{
					Helper.Content.InvalidateCache ((asset) =>
						asset.AssetNameEquals ("LooseSprites\\Cursors"));
					pendingInvalidate = false;
				}
			}, (Game1.ticks < lastInvalidate + 30) ? 500 : 0);
			lastInvalidate = Game1.ticks;
		}
	}
}
