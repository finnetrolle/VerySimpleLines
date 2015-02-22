using UnityEngine;
using System.Collections;

public class GoogleSingletonInitializer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PreferencesSingleton.Instance.SignIn ();

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play() 
	{
		Application.LoadLevel ("GameScene");
	}

	public void Leaderboard()
	{
		PreferencesSingleton.Instance.ShowLeaderboardUI ();
	}

	public void Achievements()
	{
		PreferencesSingleton.Instance.ShowAchievementsUI ();
	}
}
