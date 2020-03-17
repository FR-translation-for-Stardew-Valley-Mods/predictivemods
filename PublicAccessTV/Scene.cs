using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.IO;
using System.Media;

namespace PublicAccessTV
{
	public class Scene
	{
		protected static IModHelper Helper => ModEntry._Helper;
		protected static IMonitor Monitor => ModEntry._Monitor;
		protected static Type CustomTVMod => ModEntry.CustomTVMod;

		public string Message;
		public TemporaryAnimatedSprite Background;
		public TemporaryAnimatedSprite Overlay;

		public string SoundCueName;
		public string MusicTrack = "none";
		public string SoundAsset;

		public Action BeforeAction;
		public Action AfterAction;

		private ICue SoundCue;
		private SoundPlayer SoundPlayer;

		private TV CurrentTV;
		private Channel CurrentChannel;

		public Scene (string message, TemporaryAnimatedSprite background,
			TemporaryAnimatedSprite overlay = null)
		{
			Message = message;
			Background = background;
			Overlay = overlay;
		}

		public void Run (TV tv, Channel channel)
		{
			CurrentTV = tv;
			CurrentChannel = channel;

			if (BeforeAction != null)
				BeforeAction.Invoke ();

			if (SoundCueName != null)
			{
				SoundCue = Game1.soundBank.GetCue (SoundCueName);
				if (SoundCueName == "distantTrain")
					SoundCue.SetVariable ("Volume", 100f);
				SoundCue.Play ();
			}

			Game1.changeMusicTrack (MusicTrack ?? "none", false,
				Game1.MusicContext.Event);

			if (SoundAsset != null)
			{
				string soundPath = Path.Combine (Helper.DirectoryPath,
					"assets", $"{SoundAsset}.wav");
				SoundPlayer = new SoundPlayer (soundPath);
				SoundPlayer.Play ();
			}

			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screen")
				.SetValue (Background);
			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screenOverlay")
				.SetValue (Overlay);
			Game1.drawObjectDialogue (Game1.parseText (Message));

			Game1.afterDialogues = End;
		}

		private void End ()
		{
			if (SoundCue != null)
				SoundCue.Stop (AudioStopOptions.AsAuthored);

			if (SoundPlayer != null)
				SoundPlayer.Stop ();

			if (AfterAction != null)
				AfterAction.Invoke ();
			
			CurrentChannel.RunProgram (CurrentTV);
		}
	}
}
