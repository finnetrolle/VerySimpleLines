using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;


public class PreferencesSingleton 
{
	private static volatile PreferencesSingleton instance;
	private static object syncRoot = new Object();

	public static int SETTINGS_PLAY_LOCALLY = 0;
	public static int SETTINGS_USE_GOOGLE_PLAY = 1;
	public static int SETTINGS_ASK_USER_ABOUT_GOOGLE_PLAY = 2;
	public static int SETTINGS_OFF = 0;
	public static int SETTINGS_ON = 1;

	public static string TRICKSTER_VERY_FIRST_LINE_CRUSHED = "CgkIyIKb_8gWEAIQAQ";
	public static string TRICKSTER_TRICKY_CRUSHER = "CgkIyIKb_8gWEAIQAg";
	public static string TRICKSTER_VERY_TRICKY_CRUSHER = "CgkIyIKb_8gWEAIQAw";
	public static string TRICKSTER_INCREDIBLE_TRICKSTER = "CgkIyIKb_8gWEAIQBg";
	public static string TRICKSTER_IMPOSSIBLE_TRICKSTER = "CgkIyIKb_8gWEAIQBw";
	public static string SCORES_100 = "CgkIyIKb_8gWEAIQBA";
	public static string SCORES_200 = "CgkIyIKb_8gWEAIQBQ";
	public static string SCORES_300 = "CgkIyIKb_8gWEAIQCQ";
	public static string SCORES_500 = "CgkIyIKb_8gWEAIQCg";
	public static string SCORES_750 = "CgkIyIKb_8gWEAIQCw";
	public static string SCORES_1000 = "CgkIyIKb_8gWEAIQDA";
	public static string SCORES_1250 = "CgkIyIKb_8gWEAIQDQ";
	public static string SCORES_1500 = "CgkIyIKb_8gWEAIQDg";
	public static string SCORES_2000 = "CgkIyIKb_8gWEAIQDw";
	public static string SCORES_2500 = "CgkIyIKb_8gWEAIQEA";
	public static string SCORES_3000 = "CgkIyIKb_8gWEAIQEQ";
	public static string SCORES_4000 = "CgkIyIKb_8gWEAIQEg";
	public static string SCORES_5000 = "CgkIyIKb_8gWEAIQEw";
	public static string SCORES_6000 = "CgkIyIKb_8gWEAIQFA";
	public static string SCORES_7000 = "CgkIyIKb_8gWEAIQFQ";
	public static string SCORES_8000 = "CgkIyIKb_8gWEAIQFg";
	public static string SCORES_9000 = "CgkIyIKb_8gWEAIQFw";

	public static string LEADERBOARD_ID = "CgkIyIKb_8gWEAIQCA";

	private List<string> gainedAchievements = new List<string> ();

	public bool GainAchievement(string achievementId)
	{
		bool result = false;
		if (SignedIn)
			Social.ReportProgress (achievementId, 100.0f, (bool success) => {
				result = success;
			});
		return result;
	}

	public bool ChangeGooglePlayScores()
	{
		bool result = false;
		if (SignedIn)
			Social.ReportScore (GetHighScores(), LEADERBOARD_ID, (bool success) => {
				result = success;
			});
		return result;
	}

	public bool SignedIn { get; private set;}

	private PreferencesSingleton() 
	{
		// try to fill settings
		LoadSettings ();
	}

	public bool SignIn()
	{
		if (UseGooglePlay == SETTINGS_USE_GOOGLE_PLAY)
		{
			if (SignInGooglePlay())
				SignedIn = true;
			else
				SignedIn = false;
		}
		return SignedIn;
	}

	public void ShowAchievementsUI() 
	{
		Social.ShowAchievementsUI ();
	}
	
	public void ShowLeaderboardUI()
	{
		Social.ShowLeaderboardUI ();
	}

	public bool SaveHighScores(int scores)
	{
		bool changed = false;
		if (GetHighScores() < scores)
		{
			PlayerPrefs.SetInt("HighScores", scores);
			changed = true;
		}
		ChangeGooglePlayScores (); // This feature can update locally saved high scores for old versions
		return changed;
	}

	public int GetHighScores()
	{
		if (PlayerPrefs.HasKey("HighScores"))
		{
			return PlayerPrefs.GetInt("HighScores");
		}
		return 0;
	}

	private bool SignInGooglePlay()
	{
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			.Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();
		// sign in
		bool authResult = false;
		Social.localUser.Authenticate ((bool success) => 
		{
			if (success)
			{
				Social.LoadScores("CgkIyIKb_8gWEAIQCA", (IScore[] scores) => {
					foreach (IScore score in scores)
						Debug.Log (score.userID + " " + score.ToString() + " " + score.value);
				});
				Social.LoadAchievements((IAchievement[] obj) => {
					foreach (IAchievement a in obj)
						if (a.completed)
							gainedAchievements.Add(a.id);
				});
				authResult = true;
			}
			else
			{
				authResult = false;
				Debug.Log ("auth failed");
			}
		});
		return authResult;
	}

	private void LoadSettings()
	{
		if (PlayerPrefs.HasKey ("UseGooglePlay"))
			UseGooglePlay = PlayerPrefs.GetInt ("UseGooglePlay");
		else
			UseGooglePlay = PreferencesSingleton.SETTINGS_ASK_USER_ABOUT_GOOGLE_PLAY;
		if (PlayerPrefs.HasKey ("Sounds"))
			Sounds = PlayerPrefs.GetInt("Sounds");
		else
			Sounds = PreferencesSingleton.SETTINGS_ON;
		if (PlayerPrefs.HasKey ("Music"))
			Sounds = PlayerPrefs.GetInt("Music");
		else
			Sounds = PreferencesSingleton.SETTINGS_ON;
	}

	public int UseGooglePlay { get; private set;}
	public int Sounds { get; private set;}
	public int Music { get; private set;}

	public void SetUseGooglePlay(int value)
	{
		UseGooglePlay = value;
		PlayerPrefs.SetInt ("UseGooglePlay", UseGooglePlay);
	}

	public void SetSounds(int value)
	{
		Sounds = value;
		PlayerPrefs.SetInt ("Sounds", Sounds);
	}

	public void SetMusic(int value)
	{
		Music = value;
		PlayerPrefs.SetInt ("Music", Music);
	}

	public void GainAchievements(int destroyingBallsCount, int scores) 
	{
		// ACHIEVEMENTS SECTION
		if (scores >= 100)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_100);
		if (scores >= 200)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_200);
		if (scores >= 300)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_300);
		if (scores >= 500)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_500);
		if (scores >= 750)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_750);
		if (scores >= 1000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_1000);
		if (scores >= 1250)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_1250);
		if (scores >= 1500)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_1500);
		if (scores >= 2000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_2000);
		if (scores >= 2500)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_2500);
		if (scores >= 3000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_3000);
		if (scores >= 4000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_4000);
		if (scores >= 5000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_5000);
		if (scores >= 6000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_6000);
		if (scores >= 7000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_7000);
		if (scores >= 8000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_8000);
		if (scores >= 9000)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.SCORES_9000);
		if (destroyingBallsCount > 5)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.TRICKSTER_TRICKY_CRUSHER);
		if (destroyingBallsCount >= 9)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.TRICKSTER_VERY_TRICKY_CRUSHER);
		if (destroyingBallsCount >= 17)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.TRICKSTER_INCREDIBLE_TRICKSTER);
		if (destroyingBallsCount >= 29)
			PreferencesSingleton.Instance.GainAchievement(PreferencesSingleton.TRICKSTER_IMPOSSIBLE_TRICKSTER);
	}




	public static PreferencesSingleton Instance
	{
		get 
		{
			if (instance == null) 
			{
				lock (syncRoot) 
				{
					if (instance == null) 
						instance = new PreferencesSingleton();
				}
			}
			
			return instance;
		}
	}

}
