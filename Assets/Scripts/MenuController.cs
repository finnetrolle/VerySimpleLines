using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	public Text highScoreText;

	public void Play()
	{
		Application.LoadLevel ("GameScene");
	}

	void Awake()
	{
		if (!PlayerPrefs.HasKey("HighScores"))
			PlayerPrefs.SetInt("HighScores", 0);
		highScoreText.text = "High score: " + PlayerPrefs.GetInt("HighScores");
	}
}
