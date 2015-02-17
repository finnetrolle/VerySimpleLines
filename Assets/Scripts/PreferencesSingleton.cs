using UnityEngine;
using System.Collections;
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
	public static string SCORES_ONE_HUNDRED = "CgkIyIKb_8gWEAIQBA";
	public static string SCORES_MASTER = "CgkIyIKb_8gWEAIQBQ";
	public static string SCORES_GURU = "CgkIyIKb_8gWEAIQCQ";

	public static string LEADERBOARD_ID = "CgkIyIKb_8gWEAIQCA";

	public void GainAchievement(string achievementId)
	{
		bool result = false;
		if (SignedIn())
			Social.ReportProgress (achievementId, 100.0f, (bool success) => {
				result = success;
			});
		return result;
	}

	public bool ChangeGooglePlayScores()
	{
		bool result = false;
		if (SignedIn())
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

	private bool SingInGooglePlay()
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
						Debug.Log (a.ToString());
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
