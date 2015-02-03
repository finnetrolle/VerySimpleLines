using UnityEngine;
using System.Collections;



public class AudioController : MonoBehaviour {

	public enum SoundClip
	{
		ERROR,
		GAMEOVER,
		BALL_SELECT,
		LINE_DESTROYED
	}

	public AudioClip errorClip;
	public AudioClip gameOverClip;
	public AudioClip ballSelectClip;
	public AudioClip lineDestroyedClip;

	private AudioSource player;

	void Start()
	{
		player = GetComponent<AudioSource> ();
	}

	public void Play(SoundClip soundClip)
	{
		player.Stop ();
		switch (soundClip)
		{
			case SoundClip.BALL_SELECT: player.clip = ballSelectClip; break;
			case SoundClip.ERROR: player.clip = errorClip; break;
			case SoundClip.GAMEOVER: player.clip = gameOverClip; break;
			case SoundClip.LINE_DESTROYED: player.clip = lineDestroyedClip; break;
		}
		player.Play ();
	}


}
