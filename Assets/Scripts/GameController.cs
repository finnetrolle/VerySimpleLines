using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using AStar;



public class GameController : MonoBehaviour {

	public GooglePlayController google;

	// Inspector-based publics
	public Camera mainCamera;
	public AudioController audio;
	public BallBuilderController ballBuilder;
	public GameObject floorTilePrefab;
	public GameObject gameOverObjects;

	// GUI Text objects
	public Text scoreText;
	public Text gameOverScoreText;

	// game field settings 
	public int verticalSize = 9;
	public int horizontalSize = 9;
	public int xOffset;
	public int yOffset;

	// moving privates
	private BallController ballToMove = null;
	private FloorTileController floorToMove = null;
	private bool moving = false;

	// game field privates
	private FloorTileController[,] fields;
	private BallController[,] balls;

	// color stack 
	// Todo: implement this fucking color stack
	private BallColor[] nextColors = new BallColor[3];
	private BallController[] nextColorsBalls = new BallController[3];

	// scores!!!
	private int scores = 0;
	private Condition condition = Condition.IDLE;
	private List<BallController> nextBalls;

	//
	private List<BallController> ballsToDestroy = new List<BallController>();

	private enum Condition
	{
		IDLE,
		BALL_SELECTED,
		BALL_MOVING,
		GAME_OVER
	}

	// Use this for initialization
	void Awake () {
		float d1 = Time.time;
		InitFloorTileController ();
		float d2 = Time.time;
		InitBalls ();
		float d3 = Time.time;
		Debug.Log ("Times: " + (d2-d1) + ", " + (d3-d2));
		gameOverObjects.SetActive (false);
		float h2w = (float)(Screen.height) / (float)(Screen.width);
		float camSize = 4.6f * h2w; // magic 4.6 calculated for android. Deal with it!
		mainCamera.orthographicSize = camSize;
		condition = Condition.IDLE;
	}

	/// <summary>
	/// Method goes to main menu (nice way to use SendMessage)
	/// </summary>
	public void GoToMenu()
	{
		GameOver ();
		Application.LoadLevel ("MainMenuScene");
	}

	public void GoToAchievements()
	{
		//GameOver ();
		google.ShowAchievementsUI ();
	}

	public void GoToLeaderboard()
	{
		//GameOver ();
		google.ShowLeaderboardUI ();
	}

	private void addBallToDestroyList(BallController ball) 
	{
		ballsToDestroy.Add (ball);
	}

	private void DestroyBallsToDestroy()
	{
		foreach (BallController ball in ballsToDestroy)
			ball.ActivateBallDestruction();
		ballsToDestroy.Clear();
	}

	/// <summary>
	/// Method inits balls array
	/// </summary>
	void InitBalls ()
	{
		balls = new BallController[verticalSize, horizontalSize];
		for (int x = 0; x < horizontalSize; ++x)
			for (int y = 0; y < verticalSize; y ++)
			{
				balls[x,y] = null;
			}
		nextBalls = PlaceNextBalls (5, balls, horizontalSize, verticalSize);
		ActivateNextBalls (nextBalls, balls, horizontalSize, verticalSize);
		nextBalls = PlaceNextBalls (3, balls, horizontalSize, verticalSize);
		//dropBalls (5, null);
	}

	/// <summary>
	/// Method stops the game
	/// </summary>
	void GameOver()
	{
		audio.Play (AudioController.SoundClip.GAMEOVER);
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

		google.WriteScore (scores);
	}

	List<Point> GetFreeCells(BallController[,] balls, int maxX, int maxY)
	{
		List<Point> result = new List<Point> ();
		for (int x = 0; x < maxX; ++x)
			for (int y = 0; y < maxY; ++y)
				if (balls[x, y] == null)
					result.Add(new Point(x, y));
		return result;
	}

	List<BallController> PlaceNextBalls(int ballsCount, BallController[,] balls, int maxX, int maxY, bool autoActivate = false)
	{
		List<Point> freeCells = GetFreeCells (balls, maxX, maxY);
		if (freeCells.Count <= ballsCount)
		{
			// Signal to gameover
			Debug.Log (freeCells.Count + " " + ballsCount);
			return null;
		}
		List<BallController> result = new List<BallController> ();
		int numberToAdd = ballsCount;
		while (numberToAdd > 0)
		{
			int index = Random.Range (0, freeCells.Count);
			Point position = freeCells[index];
			freeCells.RemoveAt(index);
			BallController bc = ballBuilder.InstantiateBall((BallColor)(Random.Range (0, 6)), position.X, position.Y, xOffset, yOffset, this);
			result.Add(bc);
			if (autoActivate)
			{
				ActivateBallAndCheckDestroySolution(bc);
				//balls[position.X, position.Y] = bc;
				//bc.ActivateBall();
			}
			numberToAdd --;
		}
		return result;
	}

	void ActivateBallAndCheckDestroySolution(BallController bc) 
	{
		balls [bc.fieldX, bc.fieldY] = bc;
		bc.ActivateBall ();
		ClearLines (bc.fieldX, bc.fieldY);
	}

	void ActivateNextBalls(List<BallController> newBalls, BallController[,] balls, int maxX, int maxY)
	{
		int reCreate = 0;
		// for every newBall check space availability
		foreach (BallController bc in newBalls)
		{
			if (balls[bc.fieldX, bc.fieldY] == null)
			{
				ActivateBallAndCheckDestroySolution(bc);
				//balls[bc.fieldX, bc.fieldY] = bc;
				//bc.ActivateBall();
			}
			else 
			{
				reCreate ++;
				Destroy(bc.gameObject);
			}
		}
		// if have balls to repos - repos
		PlaceNextBalls (reCreate, balls, maxX, maxY, true);
	}

	/*
	/// <summary>
	/// Method spawns balls on game field
	/// </summary>
	/// <returns><c>true</c>, if balls was dropped, <c>false</c> otherwise.</returns>
	/// <param name="count">Number of balls to spawn</param>
	/// <param name="colorStack">Link to a color stack for spawn</param>
	bool dropBalls(int count, BallColor[] colorStack)
	{
		if (GetFreeSpace() <= count)
		{
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
				toPlace --;
				BallColor color;
				if (colorStack == null)
					color = (BallColor)Random.Range (0, 6);
				else
					color = colorStack[toPlace];
				balls[x,y] = ballBuilder.InstantiateBall(color, x, y, xOffset, yOffset, this);
				ClearLines(x, y);
			}
		}
		return true;
	}
	*/

	/// <summary>
	/// Gets the free space on game field
	/// </summary>
	/// <returns>The free cells count</returns>
	int GetFreeSpace()
	{
		int cnt = 0;
		for (int x = 0; x < horizontalSize; ++x)
			for (int y = 0; y < verticalSize; ++y)
				if (balls[x, y] != null)
					cnt ++;
		return verticalSize * horizontalSize - cnt;
	}

	/// <summary>
	/// Inits the floor tile controller array (Game field).
	/// </summary>
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


	/// <summary>
	/// Handling Ball click
	/// </summary>
	/// <param name="x">The x coordinate of ball</param>
	/// <param name="y">The y coordinate of ball</param>
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
			audio.Play(AudioController.SoundClip.BALL_SELECT);
		}
	}

	/// <summary>
	/// Handling ball moving complete event
	/// </summary>
	public void BallMovingComplete()
	{
		MoveBall (ballToMove.fieldX, ballToMove.fieldY, floorToMove.fieldX, floorToMove.fieldY);
		ballToMove.fieldX = floorToMove.fieldX;
		ballToMove.fieldY = floorToMove.fieldY;
		moving = false;
		ballToMove.SetFocus (false);
		ResetFieldSelections ();
		if (!ClearLines (ballToMove.fieldX, ballToMove.fieldY))
		{
			// we here when balls not cleared and it's time to activate preBalls
			ActivateNextBalls(nextBalls, balls, horizontalSize, verticalSize);
			// and create new nextBalls
			nextBalls = PlaceNextBalls(3, balls, horizontalSize, verticalSize);
			if (nextBalls == null)
				GameOver();
			//	dropBalls (3, null); // Todo: change null to preselector
		}
		ballToMove = null;
		floorToMove = null;
	}

	/// <summary>
	/// Method tries to remove one line - horizontal, vertical or diagonal, based on deltas (in both directions)
	/// </summary>
	/// <returns>Length of a line if it is bigger than 4</returns>
	/// <param name="point">Center (base) point</param>
	/// <param name="dx1">Delta X for 1st direstion</param>
	/// <param name="dy1">Delta Y for 1st direstion</param>
	/// <param name="dx2">Delta X for 2nd direstion</param>
	/// <param name="dy2">Delta Y for 2nd direstion</param>
	int RemoveLine(Point point, int dx1, int dy1, int dx2, int dy2)
	{
		List<Point> a = GetSameBallsInLine (point, dx1, dy1, horizontalSize, verticalSize);
		List<Point> b = GetSameBallsInLine (point, dx2, dy2, horizontalSize, verticalSize);
		int drop = a.Count + b.Count + 1;
		if (drop >= 5)
		{
			RemoveBalls(a);
			RemoveBalls(b);
			return drop;
		}
		return 0;
	}

	/// <summary>
	/// Method tries to find in horizontal, vertical and diagonal lines line of 5 and more same balls and delete them
	/// </summary>
	/// <returns><c>true</c>, if lines was cleared, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate of center (base) point</param>
	/// <param name="y">The y coordinate of center (base) point</param>
	bool ClearLines(int x, int y)
	{
		Point point = new Point (x, y);
		int alldrop = 
				RemoveLine(point, 0, -1, 0, 1) +
				RemoveLine(point, -1, 0, 1, 0) +
				RemoveLine(point, 1, 1, -1, -1) +
				RemoveLine(point, -1, 1, 1, -1);
		if (alldrop > 0)
		{
			// destroy starter ball
			BallController bc = balls[x, y];
			balls[x, y] = null;
			addBallToDestroyList(bc);
			DestroyBallsToDestroy();
			//bc.ActivateBallDestruction();
			//Destroy(bc.gameObject);

			int plus = (alldrop - 4) * alldrop;
			if (scores == 0)
				google.ActivateAchievementVFLC();
			this.scores += plus;
			// ACHIEVEMENTS SECTION
			if (this.scores >= 100)
				google.ActivateAchievement100();
			if (this.scores >= 200)
				google.ActivateAchievement200();
			if (this.scores >= 300)
				google.ActivateAchievement300();
			if (alldrop > 5)
				google.ActivateAchievementTC();
			if (alldrop >= 9)
				google.ActivateAchievementVTC();
			if (alldrop >= 17)
				google.ActivateAchievementInT();
			if (alldrop >= 29)
				google.ActivateAchievementImT();


			UpdateUI ();
			audio.Play(AudioController.SoundClip.LINE_DESTROYED);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Updates the UI (scores text)
	/// </summary>
	void UpdateUI()
	{
		scoreText.text = "Score: " + scores;
	}

	/// <summary>
	/// Removes the balls from balls[,] array
	/// </summary>
	/// <param name="points">List of coordinates of balls to remove</param>
	void RemoveBalls(List<Point> points)
	{
		foreach(Point p in points)
		{
			BallController bc = balls[p.X, p.Y];
			balls[p.X, p.Y] = null;
			addBallToDestroyList(bc);
			//bc.ActivateBallDestruction();
			//Destroy (bc.gameObject);
		}
	}

	/// <summary>
	/// Method returns list of coordinates of balls with same color in direction based on dx,dy
	/// </summary>
	/// <returns>List of Points</returns>
	/// <param name="start">base point</param>
	/// <param name="dx">delta x</param>
	/// <param name="dy">delta y</param>
	/// <param name="maxX">maximal x</param>
	/// <param name="maxY">maximal y</param>
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

	/// <summary>
	/// Method handles user's click on floor tile
	/// </summary>
	/// <param name="x">The x coordinate of tile</param>
	/// <param name="y">The y coordinate of tile</param>
	public void FloorTileClickHandler(int x, int y)
	{
		if ((!moving) && (ballToMove != null)) 
		{
			if (balls[x, y] != null)
			{
				return;
			}
			fields [x, y].transform.localScale = new Vector3 (1f, 0.01f, 1f);
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
				audio.Play (AudioController.SoundClip.ERROR);
			}
			else
			{
				// animate!
				ballToMove.AnimatePath(path);
				moving = true;
			}
		}
	}

	/// <summary>
	/// Method uses A* algorhitm from built-in AStar namespace to find best path
	/// </summary>
	/// <returns>Shortest path from Start point to End point</returns>
	/// <param name="fx">Start point x</param>
	/// <param name="fy">Start point y</param>
	/// <param name="tx">End point x</param>
	/// <param name="ty">End point y</param>
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

	/// <summary>
	/// Resets the fields scale (selection)
	/// </summary>
	void ResetFieldSelections()
	{
		Vector3 normalScale = new Vector3 (1f, 0.01f, 1f);
		foreach (FloorTileController ftc in fields)
		{
			ftc.transform.localScale = normalScale;
		}
	}

	/// <summary>
	/// Moves the ball link from one cell into another
	/// </summary>
	/// <param name="fromX">From x.</param>
	/// <param name="fromY">From y.</param>
	/// <param name="toX">To x.</param>
	/// <param name="toY">To y.</param>
	void MoveBall(int fromX, int fromY, int toX, int toY)
	{
		balls [toX, toY] = balls [fromX, fromY];
		balls [fromX, fromY] = null;
	}

}










