using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    List<AudioSource> effectSources = new List<AudioSource>();

	public void PlaySound(string soundName)
	{
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = SoundRepository.Instance.GetClip(soundName);
		source.Play();
	}

	public void Update()
	{
		for (int i = 0; i < effectSources.Count; ++i)
		{
			if (!effectSources[i].isPlaying)
			{
				Destroy(effectSources[i]);
				effectSources.RemoveAt(i);
				--i;
			}
		}
	}
}
