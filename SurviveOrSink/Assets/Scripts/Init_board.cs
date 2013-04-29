using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battleship;

public class Init_board : MonoBehaviour 
{
	//images for links
	public GUIStyle back;
	public GUIStyle quit;
	public GUIStyle scores;
	public GUIStyle instructions;
	public GUIStyle menuPopup;
	public GUIStyle instructionsPopup;
	public GUIStyle wipPopup;
	
	//used in popup window
	private bool showPopup = false;
	private Rect rectPopup;			//rectangle for popup window
	private bool hideMenu = false;
	
	//keep track of score
	public static int score;
	
	public static GameObject[,] gameGrid = new GameObject[10,10];
	public static GameObject[,] observationGrid = new GameObject[10,10];
	public Vector2 scrollPosition = new Vector2(100,100);
	public static string messages = "Begin Game!";
	
	void OnGUI()
	{
		var threeQuartersWidth = (int)(3 * Screen.width/4);
		if( (int) Time.timeSinceLevelLoad < 3)
		{
			GUI.Label(new Rect(threeQuartersWidth,25,100,100), "BEGIN GAME!");
		}
		
		GUILayout.BeginArea(new Rect((Screen.width/2)+225,20,(Screen.width/2)-225,(Screen.height/2)-20));
		//GUILayout.BeginArea(Rect((Screen.width/2)+125,20,Screen.width/2-125,(Screen.height/2)-20),"This is the text to be displayed");
		//GUILayout.BeginArea(new Rect (100,100,100,100));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width((Screen.width/2)-225), GUILayout.Height((Screen.height/2)-20));
		GUILayout.Label(messages);
		//scrollPosition.y=1000000;//keeps the scroll at the bottom of the screen
		if (GUILayout.Button("Clear"))
            messages = "";
		GUILayout.EndScrollView();
		//if (GUILayout.Button("Add More Text"))
            //messages += "\nHere is another line";
		 GUILayout.EndArea ();
		
		//scrollPosition = GUI.BeginScrollView(new Rect((Screen.width/2)+125,20,Screen.width/2-125,(Screen.height/2)-20),scrollPosition,new Rect((Screen.width/2)+125,20,Screen.width/2-125,(Screen.height/2)-20));
		//scrollPosition = GUI.BeginScrollView(new Rect((Screen.width/2)+125,20,(Screen.width/2)-20,(Screen.height/2)-20),scrollPosition,new Rect((Screen.width/2)+125,20,Screen.width/2-125,(Screen.height/2)-20),false,true);
		//GUI.Label(new Rect((Screen.width/2)+125,20,Screen.width/2-125,(Screen.height/2)-20), messages);
		//GUI.EndScrollView();
		if (Event.current.Equals(Event.KeyboardEvent("escape")) || LocalBattle.gameOver) {
			if (LocalBattle.gameOver)
				PlayerPrefs.SetInt("currentplayer", score);
			popupToggle();
		}
		rectPopup = new Rect((int)Screen.width/2-480,80,960,520);
		if (showPopup == true) {
			rectPopup = GUI.Window(0, rectPopup, PopupWindow, "");
			GUI.FocusWindow(0);
		}
	}
	
	// Use this for initialization
	void Start ()
    {
        Material emptyBoard_transparent = (Material)Resources.Load("Empty_Transparent", typeof(Material));
        Material emptyBoard_solid = (Material)Resources.Load("Empty_Solid", typeof(Material));
		
		//draw grids using individual cubes and store in dictionary (NxN)
		for(int rows = 0; rows < 10; ++rows)
		{
			for(int cols = 0; cols < 10; ++cols)
			{
				
				//gameGrid
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.renderer.material = emptyBoard_transparent;
				cube.transform.position = new Vector3(rows, (float)-1, cols);
				cube.transform.localScale += new Vector3(0, (float)-.5, 0);
                cube.layer = LayerMask.NameToLayer("BoardPiece");
                cube.tag = "BoardPiece";
                cube.name = "" + rows + "," + cols;
				gameGrid[rows,cols] = cube;
				
				
				
				//observationGrid
				GameObject smallCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                smallCube.renderer.material = emptyBoard_solid;
                smallCube.renderer.material.color = Color.white;
                smallCube.layer = LayerMask.NameToLayer("EnemyBoardPiece");
                smallCube.tag = "EnemyBoardPiece";
                smallCube.name = "" + rows + "," + cols;
				double temp = rows+35-rows*.1;
				float x_loc = (float)temp;
				float y_loc = (float)(cols - cols*.1);
				smallCube.transform.position = new Vector3(x_loc, y_loc, 5);
				smallCube.transform.localScale += new Vector3((float)-.1, (float)-.1, (float)-.1);
				observationGrid[rows, cols] = smallCube;
				
			}
		}
		
		//gameGrid[16].renderer.material.color = Color.white;
        var go1 = new GameObject("Player 1");
        var op1 = go1.AddComponent<HumanPlayer>();

        var go2 = new GameObject("Player 2");
        var op2 = go2.AddComponent<RandomOpponent>();

        var battle = new GameObject("LocalBattle");
        battle.AddComponent<LocalBattle>().initialize(op1, op2, new Size(10, 10));
	
					
		//Keep track of score
		score = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	public GameObject[,] getObservationGrid()
	{
		return observationGrid;	
	}
	
	public GameObject[,] getGameGrid()
	{
		return gameGrid;	
	}
	
	//GUI - Separate window that pops up
	private void PopupWindow(int windowID) {
		//used in determining button size & position
		int buttonH = 51;						//height of button
		int buttonW = 276;						//width of button
		int buttonSpace = 15;					//space between buttons
		int buttonHoriz = (int) (960-276)/2;	//x-coordinate of button
		int buttonVert = 100;					//y-coordinate of button
		
		if (hideMenu == false) {
			GUI.Box(new Rect(0,0,960,520),"",menuPopup);
			
			//High scores button
			if (GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",scores)) {
				Application.LoadLevel("Scoreboard");
			}
			
			//Instructions button
			buttonVert += (buttonH+buttonSpace);
			if (GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",instructions)) {
				menuToggle();
			}
			
			//Quit button
			buttonVert += (buttonH+buttonSpace);
			if (GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",quit)) {
				Application.LoadLevel("MainMenu");
			}
		}
		else
			GUI.Box(new Rect(0,0,960,520),"",instructionsPopup);
		
		//Back button
		if (GUI.Button(new Rect(850,460,92,37),"",back)) {
			if (hideMenu == true)
				menuToggle();
			else
				popupToggle();
		}			
	}
	//GUI - toggles popup window on/off
	private void popupToggle() {
		if (showPopup == true)
			showPopup = false;
		else
			showPopup = true;
	}
	//GUI - toggles menu popup on/off
	private void menuToggle() {
		if (hideMenu == true)
			hideMenu = false;
		else
			hideMenu = true;
	}
}
