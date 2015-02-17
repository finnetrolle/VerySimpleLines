using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	public Text highScoreText;

	public void Play()
	{
		Application.LoadLevel ("GameScene");
	}

	void Start()
	{
		PreferencesSingleton.Instance.SignIn ();
		highScoreText.text = "High score: " + PreferencesSingleton.Instance.GetHighScores ();
	}
}
