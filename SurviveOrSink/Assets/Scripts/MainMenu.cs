using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	// Almost everything GUI-related goes here
	void OnGUI() {
		int halfWidth = (int) Screen.width/2;
		//int halfHeight = (int) Screen.height/2;
		
		//Creating random buttons
		if(GUI.Button(new Rect(halfWidth-200,100,400,100),"Test Button")) {
			Application.LoadLevel("TestLevel");
		}
		if(GUI.Button(new Rect(halfWidth-200,210,400,100),"Another Test Button")) {
			Application.LoadLevel("TestLevel");
		}
		if(GUI.Button(new Rect(halfWidth-200,320,400,100),"Yet Another Test Button")) {
			Application.LoadLevel("TestLevel");
		}
		if(GUI.Button(new Rect(halfWidth-200,430,400,100),"Still a Test Button")) {
			Application.LoadLevel("TestLevel");
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
