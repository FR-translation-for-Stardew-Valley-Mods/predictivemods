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

		public CursorEditor ()
		{
			cursor = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "cursor.png"));
		}

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.DataType == typeof (Texture2D) &&
				asset.AssetNameEquals ("LooseSprites\\Cursors");
		}

		public void Edit<_T> (IAssetData asset)
		{
			IAssetDataForImage editor = asset.AsImage ();
			editor.PatchImage (cursor, targetArea: new Rectangle (112, 0, 16, 16));
		}

		public bool Active =>
			ModEntry.OrbHovered || ModEntry.OrbsIlluminated > 0;

		public void Apply ()
		{
			if (Active)
				Game1.mouseCursor = 7;
			else if (Game1.mouseCursor == 7)
				Game1.mouseCursor = 0;
		}

		internal void BeforeRenderMenu ()
		{
			// When active, prevent the normal software cursor from being drawn
			// by the menu.
			if (Active && !Game1.options.hardwareCursor)
				Game1.mouseCursorTransparency = 0f;
		}

		internal void AfterRenderMenu (SpriteBatch b)
		{
			// When active, draw the special cursor instead. Restoring the
			// regular mouseCursorTransparency is apparently not helpful.
			if (Active && !Game1.options.hardwareCursor)
			{
				b.Draw (Game1.mouseCursors,
					new Vector2 (Game1.getMouseX (), Game1.getMouseY ()),
					Game1.getSourceRectForStandardTileSheet
						(Game1.mouseCursors, 7, 16, 16),
					Color.White, 0f, Vector2.Zero,
					4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
			}
		}
	}
}
