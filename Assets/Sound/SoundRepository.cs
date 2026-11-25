using UnityEngine;

public class SoundRepository : Singleton<SoundRepository>
{
    SoundCollection collection;

	public void Awake()
	{
		collection = Resources.Load<SoundCollection>("SoundCollection");
	}

	public SoundScriptableObject GetClip(string clipName)
	{
		foreach (SoundScriptableObject clip in collection.sounds)
		{
			if (clip.name == clipName)
			{
				return clip;
			}
		}
		return null;
	}
}
