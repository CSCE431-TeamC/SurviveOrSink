using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour {
	public GUISkin mySkin;
	public Texture2D scoreboard_title;
	public GUIStyle background;
	
	//images for links
	public GUIStyle quit;
	public GUIStyle back;
	public GUIStyle clear;
	
	//used in determining box size & position
	private int boxH;		//height of button
	private int boxW;		//width of button
	private int boxSpace;	//space between buttons
	private int boxHoriz;	//x-coordinate of button
	private int boxVert;	//y-coordinate of button
	
	void OnGUI() {
		//initializing...
		boxH = 65;
		boxW = 200;
		boxSpace = 20;
		boxHoriz = (int) Screen.width/2-boxW/2;
		boxVert = 135;
		GUI.skin = mySkin;
		
		//Title
		GUI.Box(new Rect(0,10,Screen.width,103),scoreboard_title);
		
		//Background
		GUI.Box(new Rect(Screen.width/2-480,105,960,520),"",background);
		
		//High Scores
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("first", "Player"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("first", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("second", "Player"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("second", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("third", "Player"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("third", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("fourth", "Player"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("fourth", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("fifth", "Player"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("fifth", 0).ToString());
		
		//Links
		if(GUI.Button(new Rect(Screen.width/2-138,550,276,51),"",clear)) {
			PlayerPrefs.DeleteAll();
		}
		if(GUI.Button(new Rect(0,Screen.height-47,102,47),"",back)) {
			Application.LoadLevel("Game");
		}
		if(GUI.Button(new Rect(Screen.width-276,Screen.height-51,276,51),"",quit)) {
			Application.LoadLevel("MainMenu");
		}
		
	}
	
	//creates new y-coordinate for next box
	private void updateBoxVert() {
		boxVert += (boxH+boxSpace);
	}
}
