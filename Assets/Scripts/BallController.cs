using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BallColor 
{
	BALL_RED,
	BALL_GREEN,
	BALL_BLUE,
	BALL_YELLOW,
	BALL_TEAL,
	BALL_ORANGE,
	BALL_VIOLET
}

public class BallController : MonoBehaviour {

	public Animator animator;
	public BallColor ballColor;
	public int fieldX;
	public int fieldY;
	public GameController gameController;

	private bool moving = false;
	private List<Vector3> path = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		if (moving)
		{
			if (path.Count > 0)
			{
				transform.position = Vector3.Lerp(transform.position, path[0], 10f * Time.deltaTime);
				if (Vector3.Distance(transform.position, path[0]) <= 0.2f )
				{
					transform.position = path[0];
					path.RemoveAt(0);
				}
			}
			else
			{
				moving = false;
				gameController.BallMovingComplete();
			}
		}
	}

	public void AnimatePath(List<Vector3> path)
	{
		this.path = path;
		moving = true;
	}

	void OnMouseDown()
	{
		//Debug.Log (ballColor + " ball clicked at [" + fieldX + "," + fieldY + "]");
		gameController.BallClickHandler (fieldX, fieldY);
	}
}
