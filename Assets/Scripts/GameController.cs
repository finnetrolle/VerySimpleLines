using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using AStar;



public class GameController : MonoBehaviour {

	public GameObject floorTilePrefab;
	public GameObject BallPrefab;
	public GameObject gameOverObjects;

	public Text scoreText;
	public Image gameoverImage;
	public Text gameOverText;
	public Text gameOverScoreText;

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


	private int scores = 0;

	// Use this for initialization
	void Start () {
		InitFloorTileController ();
		InitBalls ();
		gameOverObjects.SetActive (false);
	}

	public void GoToMenu()
	{
		GameOver ();
		Application.LoadLevel ("MainMenuScene");
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

	void GameOver()
	{
		int hs = 0;
		if (PlayerPrefs.HasKey("HighScores"))
		{
			hs = PlayerPrefs.GetInt("HighScores");
			if (scores > hs)
			{
				PlayerPrefs.SetInt ("HighScores", scores);
			}
		}
		else 
		{
			PlayerPrefs.SetInt ("HighScores", scores);
			hs = scores;
		}
		// change UI
		gameOverObjects.SetActive (true);

		gameOverScoreText.text = "Your score: " + scores + "\nHigh score: " + hs;
	}

	bool dropBalls(int count, BallColor[] colorStack)
	{
		if (GetFreeSpace() <= count)
		{
			//Debug.Log ("DROP FAILED");
			GameOver();
			return false;
		}

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
				//Debug.Log ("Ball dropped at [" + x + "," + y + "]");
				bc.gameController = this;
				if (colorStack == null)
					bc.ballColor = (BallColor)Random.Range(0, 6);
				else
					bc.ballColor = colorStack[toPlace];
				SetBallMaterial(bc);
				balls[x,y] = bc;
				ClearLines(x, y);
			}
		}
		return true;
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
		{
			if (ballToMove != null)
			{
				ballToMove.SetFocus(false);
			}
			ballToMove = balls [x, y];
			ballToMove.SetFocus(true);
		}
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
		//Debug.Log ("Ball completes his path");
		int fx = ballToMove.fieldX;
		int fy = ballToMove.fieldY;
		int tx = floorToMove.fieldX;
		int ty = floorToMove.fieldY;
		ballToMove.fieldX = tx;
		ballToMove.fieldY = ty;
		balls[tx, ty] = balls[fx, fy];
		balls [fx, fy] = null;
		moving = false;
		ballToMove.SetFocus (false);
		ballToMove = null;
		floorToMove = null;
		ResetFieldSelections ();

		// clear if 5 in line
		if (!ClearLines (tx, ty))
			dropBalls (3, null); // Todo: change null to preselector
	}

	bool ClearLines(int x, int y)
	{
		bool ballsCleared = false;
		Point point = new Point (x, y);
//		BallColor color = balls [x, y].ballColor;
		// Todo: create checking method
		List<Point> a = null;
		List<Point> b = null;
		int alldrop = 0;
		int drop = 0;
		// Test vert
		a = GetSameBallsInLine (point, 0, -1, horizontalSize, verticalSize);
		b = GetSameBallsInLine (point, 0, 1, horizontalSize, verticalSize);
		drop = a.Count + b.Count + 1;
		if (drop >= 5)
		{
			ballsCleared = true;
			alldrop += drop;
			RemoveBalls(a);
			RemoveBalls(b);
		}
		// Test Horiz
		a = GetSameBallsInLine (point, -1, 0, horizontalSize, verticalSize);
		b = GetSameBallsInLine (point, 1, 0, horizontalSize, verticalSize);
		drop = a.Count + b.Count + 1;
		if (drop >= 5)
		{
			ballsCleared = true;
			alldrop += drop;
			RemoveBalls(a);
			RemoveBalls(b);
		}
		// Test Diag \
		a = GetSameBallsInLine (point, 1, 1, horizontalSize, verticalSize);
		b = GetSameBallsInLine (point, -1, -1, horizontalSize, verticalSize);
		drop = a.Count + b.Count + 1;
		if (drop >= 5)
		{
			ballsCleared = true;
			alldrop += drop;
			RemoveBalls(a);
			RemoveBalls(b);
		}
		// Test Diag /
		a = GetSameBallsInLine (point, -1, 1, horizontalSize, verticalSize);
		b = GetSameBallsInLine (point, 1, -1, horizontalSize, verticalSize);
		drop = a.Count + b.Count + 1;
		if (drop >= 5)
		{
			ballsCleared = true;
			alldrop += drop;
			RemoveBalls(a);
			RemoveBalls(b);
		}
		//Debug.Log ("DROPPED: " + alldrop);
		if (ballsCleared)
		{
			int mod = alldrop - 4; //(X - 5 + 1)
			int pts = alldrop * mod;
			this.scores += pts;
			UpdateUI ();
			BallController bc = balls[x, y];
			balls[x, y] = null;
			Destroy(bc.gameObject);
		}
		return ballsCleared;
	}

	void UpdateUI()
	{
		scoreText.text = "Score: " + scores;
	}

	void RemoveBalls(List<Point> points)
	{
		foreach(Point p in points)
		{
			BallController bc = balls[p.X, p.Y];
			balls[p.X, p.Y] = null;
			Destroy (bc.gameObject);
		}
	}

	List<Point> GetSameBallsInLine(Point start, int dx, int dy, int maxX, int maxY)
	{
		List<Point> result = new List<Point> ();
		BallColor color = balls [start.X, start.Y].ballColor;
		Point next = new Point (start.X + dx, start.Y + dy);
		while ((next.X >= 0) && (next.X < maxX) && (next.Y >= 0) && (next.Y < maxY))
		{
			if ((balls[next.X, next.Y] != null) && (balls[next.X, next.Y].ballColor == color))
				result.Add(new Point(next.X, next.Y));
			else
				break;
			next.X += dx;
			next.Y += dy;
		}
		return result;
	}

	/*
	void int GetSameBallsCount(int startx, int starty, int dx, int dy, int maxX, int maxY)
	{
		int sum = 0;
		BallColor color = balls [startx, starty].ballColor;
		int x = startx + dx;
		int y = starty + dy;
		while ((x >= 0) && (x < maxX) && (y >= 0) && (y < maxY))
		{
			if (balls[x, y].ballColor == color)
				sum++;
			else
				break;
		}
		return sum;
	}
	*/


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

			List<Vector3> path = new List<Vector3>();
			path = AStarFind(ballToMove.fieldX, ballToMove.fieldY, floorToMove.fieldX, floorToMove.fieldY);
			if (path == null)
			{
				// drop user's event chain
				floorToMove = null;
				ballToMove.SetFocus(false);
				ballToMove = null;
				moving = false;
			}
			else
			{
				// animate!
				ballToMove.AnimatePath(path);
				moving = true;
			}
		}
	}

	List<Vector3> AStarFind(int fx, int fy, int tx, int ty)
	{
		// create field
		int[,] field = new int[horizontalSize, verticalSize];
		for(int x = 0; x < horizontalSize; ++x)
			for (int y = 0; y < verticalSize; ++y)
		{
			field[x, y] = (balls[x, y] == null) ? 0 : 1 ;
		}
		// do find
		List<AStar.Point> path = AStar.AStar.Find (new AStar.Point (fx, fy), new AStar.Point (tx, ty), AStar.AStar.GenerateField (field));
		if (path == null)
			return null;
		List<Vector3> result = new List<Vector3> ();
		foreach(AStar.Point p in path)
		{
			result.Add(new Vector3(p.X + xOffset, 0f, p.Y + yOffset));
		}
		return result;
	}

	void PrintPath(List<Vector3> path, int fromx, int fromy, int tox, int toy)
	{
		//Debug.Log ("Printing path from [" + fromx + "," + fromy + "] to [" + tox + "," + toy + "]:");
		//foreach(Vector3 v in path)
		//{
			//Debug.Log(">> [" + v.x + "," + v.z + "]");
		//}
	}

	void ResetFieldSelections()
	{
		Vector3 normalScale = new Vector3 (1f, 0.01f, 1f);
		foreach (FloorTileController ftc in fields)
		{
			ftc.transform.localScale = normalScale;
		}
	}
	
	bool IsFieldEmpty(int fx, int fy)
	{
		return false;
	}
}










