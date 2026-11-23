using UnityEngine;

public class SoundRepository : Singleton<SoundRepository>
{
    SoundCollection collection;

	public void Awake()
	{
		collection = Resources.Load<SoundCollection>("SoundCollection");
	}

	public AudioClip GetClip(string clipName)
	{
		foreach (AudioClip clip in collection.sounds)
		{
			if (clip.name == clipName)
			{
				return clip;
			}
		}
		return null;
	}
}
