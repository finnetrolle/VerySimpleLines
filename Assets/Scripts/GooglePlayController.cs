using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GooglePlayController : MonoBehaviour {

	// Use this for initialization
	void Start () {

		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
				.Build();
		
		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();
		SingIn ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SingIn()
	{
		Social.localUser.Authenticate ((bool success) => 
		{
			if (success)
			{
				Debug.Log ("auth success");
				Debug.Log (Social.localUser.userName);
				Social.LoadScores("CgkIyIKb_8gWEAIQCA", (IScore[] scores) => {
					foreach (IScore score in scores)
						Debug.Log (score.userID + " " + score.ToString() + " " + score.value);
				});
				Social.LoadAchievements((IAchievement[] obj) => {
					foreach (IAchievement a in obj)
						Debug.Log (a.ToString());
				});
			}
			else
				Debug.Log ("auth failed");
		});
	}

	public void ShowAchievementsUI() 
	{
		Social.ShowAchievementsUI ();
	}

	public void ShowLeaderboardUI()
	{
		Social.ShowLeaderboardUI ();
	}

	public void WriteScore(int score)
	{
		Social.ReportScore (score, "CgkIyIKb_8gWEAIQCA", (bool success) => {
			// handler... no handler =)))
		});
	}

	public void ActivateAchievementVFLC()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQAQ", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}

	public void ActivateAchievementTC()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQAg", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}

	public void ActivateAchievementVTC()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQAw", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}

	public void ActivateAchievement100()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQBA", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}

	public void ActivateAchievement200()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQBQ", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}

	public void ActivateAchievement300()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQCQ", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}

	public void ActivateAchievementInT()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQBg", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}

	public void ActivateAchievementImT()
	{
		Social.ReportProgress ("CgkIyIKb_8gWEAIQBw", 100.0f, (bool success) => {
			// handler... no handler =)
		});
	}


}
