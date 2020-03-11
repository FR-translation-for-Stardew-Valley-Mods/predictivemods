namespace ScryingOrb
{
	public class NothingExperience : Experience
	{
		protected override bool Try ()
		{
			// Unlike all other experiences, look for the offering check to fail.
			if (!IsAvailable || base.Try ())
				return false;

			ShowMessage ("nothing.message");

			return true;
		}
	}
}
