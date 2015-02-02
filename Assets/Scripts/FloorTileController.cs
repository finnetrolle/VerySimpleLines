using UnityEngine;
using System.Collections;

public class FloorTileController : MonoBehaviour {

	public int fieldX;
	public int fieldY;
	public GameController gameController = null;
	public BallColor ballColor;

	void OnMouseDown()
	{
		gameController.FloorTileClickHandler (fieldX, fieldY);
	}

	void OnMouseUp()
	{

	}

}
