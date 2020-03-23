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

		public string message;
		public TemporaryAnimatedSprite background;
		public TemporaryAnimatedSprite overlay;

		public string soundCueName;
		public string musicTrack = "none";
		public string soundAsset;

		public Action beforeAction;
		public Action afterAction;

		private ICue soundCue;
		private SoundPlayer soundPlayer;

		private TV currentTV;
		private Channel currentChannel;

		public Scene (string message, TemporaryAnimatedSprite background,
			TemporaryAnimatedSprite overlay = null)
		{
			this.message = message;
			this.background = background;
			this.overlay = overlay;
		}

		public void Run (TV tv, Channel channel)
		{
			currentTV = tv;
			currentChannel = channel;

			if (beforeAction != null)
				beforeAction.Invoke ();

			if (soundCueName != null)
			{
				soundCue = Game1.soundBank.GetCue (soundCueName);
				if (soundCueName == "distantTrain")
					soundCue.SetVariable ("Volume", 100f);
				soundCue.Play ();
			}

			Game1.changeMusicTrack (musicTrack ?? "none", false,
				Game1.MusicContext.Event);

			if (soundAsset != null)
			{
				string soundPath = Path.Combine (Helper.DirectoryPath,
					"assets", $"{soundAsset}.wav");
				soundPlayer = new SoundPlayer (soundPath);
				soundPlayer.Play ();
			}

			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screen")
				.SetValue (background);
			Helper.Reflection.GetField<TemporaryAnimatedSprite> (tv, "screenOverlay")
				.SetValue (overlay);
			Game1.drawObjectDialogue (Game1.parseText (message));

			Game1.afterDialogues = End;
		}

		private void End ()
		{
			// Calls to the Stop methods are best effort here, since none of the
			// sound cues or custom sounds used here are looping and at least
			// the first of the two is apparently cursed sometimes.
				
			if (soundCue != null)
			{
				try
				{
					soundCue.Stop (AudioStopOptions.AsAuthored);
				}
				catch (Exception)
				{}
			}

			if (soundPlayer != null)
			{
				try
				{
					soundPlayer.Stop ();
				}
				catch (Exception)
				{}
			}

			if (afterAction != null)
				afterAction.Invoke ();
			
			currentChannel.RunProgram (currentTV);
		}
	}
}
