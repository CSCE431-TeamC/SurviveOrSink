using UnityEngine;
using System.Collections;

public class MainMenu_moving : MonoBehaviour {
	public int timeInDir; //how lng the object should move in particular dir
	public float rateMove; //how fast the object should move
	private int changeDirection; //helps determine when the object should move in opposite dir
	
	// Use this for initialization
	void Start () {
		changeDirection = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float newX = Time.deltaTime * rateMove;
		int updateTime = (int) Time.timeSinceLevelLoad - changeDirection*timeInDir;

		if (updateTime < timeInDir)
			transform.Translate(newX,0,0);	
		else if (updateTime < timeInDir*2)
			transform.Translate(-newX,0,0);
		else 
			changeDirection += 2;
	}
}
