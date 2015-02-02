using UnityEngine;
using System.Collections;

public class DestroyOnClick : MonoBehaviour {

	void OnMouseDown()
	{
		Debug.Log (gameObject + " is clicked!");
	}
}
