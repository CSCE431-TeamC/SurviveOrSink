using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour {
	public GUISkin mySkin;
	public Texture2D scoreboard_title;
	public GUIStyle background;
	private bool alreadyChecked = false;
	
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
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("firstN"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("first", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("secondN"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("second", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("thirdN"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("third", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("fourthN"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("fourth", 0).ToString());
		updateBoxVert();
		GUI.Box(new Rect(boxHoriz/2,boxVert,boxW,boxH), PlayerPrefs.GetString("fifthN"));
		GUI.Box(new Rect(boxHoriz*(float)1.5,boxVert,boxW,boxH), PlayerPrefs.GetInt("fifth", 0).ToString());
		
		//Update scores if necessary
		if (LocalBattle.gameOver && PlayerPrefs.GetInt("currentplayer") >= PlayerPrefs.GetInt("fifth") && alreadyChecked == false) {
			scoreUpdate();
			alreadyChecked = true;
		}
		
		//Links
		if(GUI.Button(new Rect(Screen.width/2-138,550,276,51),"",clear)) {
			PlayerPrefs.DeleteAll();
		}
		if(GUI.Button(new Rect(Screen.width-276,Screen.height-51,276,51),"",quit)) {
			Application.LoadLevel("MainMenu");
		}
		
	}
	
	//creates new y-coordinate for next box
	private void updateBoxVert() {
		boxVert += (boxH+boxSpace);
	}
	
	//Update scoreboard
	void scoreUpdate() 
	{
		int score = PlayerPrefs.GetInt("currentplayer");
		
		if (score >= PlayerPrefs.GetInt("first") ) 
		{
			PlayerPrefs.SetString("fifthN",PlayerPrefs.GetString("fourthN"));
			PlayerPrefs.SetString("fourthN",PlayerPrefs.GetString("thirdN"));
			PlayerPrefs.SetString("thirdN",PlayerPrefs.GetString("secondN"));
			PlayerPrefs.SetString("secondN",PlayerPrefs.GetString("firstN"));
			
			PlayerPrefs.SetInt("fifth",PlayerPrefs.GetInt("fourth"));
			PlayerPrefs.SetInt("fourth",PlayerPrefs.GetInt("third"));
			PlayerPrefs.SetInt("third", PlayerPrefs.GetInt("second"));
			PlayerPrefs.SetInt("second",PlayerPrefs.GetInt("first"));
			
			PlayerPrefs.SetString("firstN",MainMenu.playerName);
			PlayerPrefs.SetInt("first",score);
		}	
		else if (score >= PlayerPrefs.GetInt("second")) 
		{
			PlayerPrefs.SetString("fifthN",PlayerPrefs.GetString("fourthN"));
			PlayerPrefs.SetString("fourthN",PlayerPrefs.GetString("thirdN"));
			PlayerPrefs.SetString("thirdN",PlayerPrefs.GetString("secondN"));
			
			PlayerPrefs.SetInt("fifth",PlayerPrefs.GetInt("fourth"));
			PlayerPrefs.SetInt("fourth",PlayerPrefs.GetInt("third"));
			PlayerPrefs.SetInt("third", PlayerPrefs.GetInt("second"));
			
			PlayerPrefs.SetString("secondN",MainMenu.playerName);
			PlayerPrefs.SetInt("second",score);
		}	
		else if (score >= PlayerPrefs.GetInt("third")) 
		{			
			PlayerPrefs.SetString("fifthN",PlayerPrefs.GetString("fourthN"));
			PlayerPrefs.SetString("fourthN",PlayerPrefs.GetString("thirdN"));
			
			PlayerPrefs.SetInt("fifth",PlayerPrefs.GetInt("fourth"));
			PlayerPrefs.SetInt("fourth",PlayerPrefs.GetInt("third"));
			
			PlayerPrefs.SetString("thirdN",MainMenu.playerName);
			PlayerPrefs.SetInt("third",score);
		}
		else if (score >= PlayerPrefs.GetInt("fourth") ) 
		{
			PlayerPrefs.SetString("fifthN",PlayerPrefs.GetString("fourthN"));		
			PlayerPrefs.SetInt("fifth",PlayerPrefs.GetInt("fourth"));	
			
			PlayerPrefs.SetString("fourthN",MainMenu.playerName);
			PlayerPrefs.SetInt("fourth",score);
		}
		else if (score >= PlayerPrefs.GetInt("fifth")) 
		{
			PlayerPrefs.SetString("fifthN",MainMenu.playerName);
			PlayerPrefs.SetInt("fifth",score);
		}
	}	
}
