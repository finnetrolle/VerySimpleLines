using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GameController : MonoBehaviour {

	public GameObject floorTilePrefab;
	public GameObject BallPrefab;

	public Material BallRed;
	public Material BallGreen;
	public Material BallBlue;
	public Material BallYellow;
	public Material BallTeal;
	public Material BallOrange;
	public Material BallViolet;

	public int verticalSize = 9;
	public int horizontalSize = 9;
	public int xOffset;
	public int yOffset;

	private BallController ballToMove = null;
	private FloorTileController floorToMove = null;
	private bool moving = false;

	private FloorTileController[,] fields;
	private BallController[,] balls;

	private BallColor[] nextColors;

	// Use this for initialization
	void Start () {
		InitFloorTileController ();
		InitBalls ();
	}

	void InitBalls ()
	{
		balls = new BallController[verticalSize, horizontalSize];
		for (int x = 0; x < horizontalSize; ++x)
			for (int y = 0; y < verticalSize; y ++)
			{
				balls[x,y] = null;
			}

		dropBalls (5, null);
	}

	void dropBalls(int count, BallColor[] colorStack)
	{
		int toPlace = count;
		while (toPlace > 0)
		{
			int x = Random.Range (0, horizontalSize);
			int y = Random.Range (0, verticalSize);
			if (balls[x, y] == null) 
			{
				// can place here
				toPlace --;
				GameObject obj = (GameObject)GameObject.Instantiate(BallPrefab, new Vector3(x + xOffset, 0.5f, y + yOffset), new Quaternion(90f, 90f, 0f, 0f));
				BallController bc = obj.GetComponent<BallController>();
				bc.fieldX = x;
				bc.fieldY = y;
				Debug.Log ("Ball dropped at [" + x + "," + y + "]");
				bc.gameController = this;
				if (colorStack == null)
					bc.ballColor = (BallColor)Random.Range(0, 6);
				else
					bc.ballColor = colorStack[toPlace];
				SetBallMaterial(bc);
				balls[x,y] = bc;
			}
		}
	}

	int GetFreeSpace()
	{
		int cnt = 0;
		for (int x = 0; x < horizontalSize; ++x)
			for (int y = 0; y < verticalSize; ++y)
				if (balls[x, y] != null)
					cnt ++;
		return verticalSize * horizontalSize - cnt;
	}

	void InitFloorTileController()
	{
		fields = new FloorTileController[horizontalSize, verticalSize];
		for (int x = xOffset; x < xOffset + horizontalSize; ++x)
		{
			for (int y = yOffset; y < yOffset + verticalSize; ++y)
			{
				GameObject obj = (GameObject)GameObject.Instantiate(floorTilePrefab, new Vector3(x, 0.0f, y), new Quaternion(90f, 0f, 0f, 0f));
				FloorTileController ftc = obj.GetComponent<FloorTileController>();
				ftc.fieldX = x - xOffset;
				ftc.fieldY = y - yOffset;
				ftc.gameController = this;
				fields[ftc.fieldX, ftc.fieldY] = ftc;
			}
		}
		ResetFieldSelections ();
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

	public void BallClickHandler(int x, int y)
	{
		if (!moving)
			ballToMove = balls [x, y];
		//if (balls[x,y] != null)
		//{
		//	Destroy(balls[x,y].gameObject);
		//	balls[x,y] = null;
		//}
		//if (GetFreeSpace() > 3)
		//	dropBalls (3, null);
		//else
		//	Debug.Log ("GAME OVER");
	}

	public void BallMovingComplete()
	{
		Debug.Log ("Ball completes his path");
		int fx = ballToMove.fieldX;
		int fy = ballToMove.fieldY;
		int tx = floorToMove.fieldX;
		int ty = floorToMove.fieldY;
		ballToMove.fieldX = tx;
		ballToMove.fieldY = ty;
		balls[tx, ty] = balls[fx, fy];
		balls [fx, fy] = null;
		moving = false;
		ballToMove = null;
		floorToMove = null;
		ResetFieldSelections ();
	}

	public void FloorTileClickHandler(int x, int y)
	{
		if ((!moving) && (ballToMove != null)) 
		{
			if (balls[x, y] != null)
			{
				return;
			}
			fields [x, y].transform.localScale = new Vector3 (0.9f, 0.01f, 0.9f);
			floorToMove = fields[x, y];
			moving = true;
			List<Vector3> path = new List<Vector3>();
			int dx = floorToMove.fieldX - ballToMove.fieldX;
			int dy = floorToMove.fieldY - ballToMove.fieldY;
			int tx = (int)ballToMove.transform.position.x;//int tx = ballToMove.fieldX;
			int ty = (int)ballToMove.transform.position.z;//int ty = ballToMove.fieldY;
			while (dx != 0)
			{
				if (dx > 0)
				{
					dx --;
					tx ++;
				}
				else
				{
					dx ++;
					tx --;
				}
				path.Add(new Vector3(tx, 0.5f, ty));
			}
			while (dy != 0)
			{
				if (dy > 0)
				{
					dy --;
					ty ++;
				}
				else
				{
					dy ++;
					ty --;
				}
				path.Add(new Vector3(tx, 0.5f, ty));
			}
			PrintPath(path, ballToMove.fieldX, ballToMove.fieldY, floorToMove.fieldX, floorToMove.fieldY);
			ballToMove.AnimatePath(path);
		}
	}

	void PrintPath(List<Vector3> path, int fromx, int fromy, int tox, int toy)
	{
		Debug.Log ("Printing path from [" + fromx + "," + fromy + "] to [" + tox + "," + toy + "]:");
		foreach(Vector3 v in path)
		{
			Debug.Log(">> [" + v.x + "," + v.z + "]");
		}
	}

	void ResetFieldSelections()
	{
		Vector3 normalScale = new Vector3 (1f, 0.01f, 1f);
		foreach (FloorTileController ftc in fields)
		{
			ftc.transform.localScale = normalScale;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool IsFieldEmpty(int fx, int fy)
	{
		return false;
	}
}
