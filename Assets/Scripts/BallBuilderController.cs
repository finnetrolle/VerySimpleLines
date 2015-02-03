using UnityEngine;
using System.Collections;

public class BallBuilderController : MonoBehaviour 
{
	public Material BallRed;
	public Material BallGreen;
	public Material BallBlue;
	public Material BallYellow;
	public Material BallTeal;
	public Material BallOrange;
	public Material BallViolet;

	public GameObject ballPrefab;

	public BallController InstantiateBall(BallColor ballColor, int x, int y, int xOffset, int yOffset, GameController gameController)
	{
		GameObject obj = (GameObject)GameObject.Instantiate(ballPrefab, new Vector3(x + xOffset, 0.5f, y + yOffset), new Quaternion(90f, 90f, 0f, 0f));
		BallController bc = obj.GetComponent<BallController> ();
		bc.fieldX = x;
		bc.fieldY = y;
		bc.gameController = gameController;
		bc.ballColor = ballColor;
		SetBallMaterial(bc);
		return bc;
	}

	void SetBallMaterial(BallController bc)
	{
		MeshRenderer renderer = bc.gameObject.GetComponent<MeshRenderer> ();
		switch (bc.ballColor)
		{
		case BallColor.BALL_RED: renderer.material = BallRed; break;
		case BallColor.BALL_GREEN: renderer.material = BallGreen; break;
		case BallColor.BALL_BLUE: renderer.material = BallBlue; break;
		case BallColor.BALL_ORANGE: renderer.material = BallOrange; break;
		case BallColor.BALL_TEAL: renderer.material = BallTeal; break;
		case BallColor.BALL_VIOLET: renderer.material = BallViolet; break;
		case BallColor.BALL_YELLOW: renderer.material = BallYellow; break;
		}
	}


}
