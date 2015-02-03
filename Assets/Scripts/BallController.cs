﻿using UnityEngine;
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
	private AudioSource audio;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		audio = GetComponent<AudioSource> ();
	}

	void FixedUpdate()
	{
		if (moving)
		{
			if (path.Count > 0)
			{
				transform.position = Vector3.Lerp(transform.position, path[0], (8f + path.Count) * Time.deltaTime);
				if (Vector3.Distance(transform.position, path[0]) <= 0.3f )
				{
					transform.position = path[0];
					path.RemoveAt(0);
					audio.Play();
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
		gameController.BallClickHandler (fieldX, fieldY);
	}

	public void SetFocus(bool focus)
	{
		if (focus)
			animator.SetTrigger ("BallSelected");
		else
			animator.SetTrigger ("BallDeselected");
	}
}
