using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	public GUISkin mySkin;
	public Texture2D logo;
	
	//images for links
	public GUIStyle play;
	public GUIStyle highscores;
	public GUIStyle instructions;
	public GUIStyle options;
	public GUIStyle ships;
	public GUIStyle credits;
	public GUIStyle quit;
	public GUIStyle back;
	
	//used in popup windows
	private bool showPopup = false;	//toggles popup window for various submenus
	private Rect rectPopup;			//rectangle for popup window
	private Rect namePopup;			//rectangle for name prompt
	public static string playerName;
	public string pName;
	public static string gameType;
	private int windowID = -1;
	
	//toggle buttons
	private bool p1human = true;
	private bool p1random = false;
	private bool p1smart = false;
	private bool p2random = true;
	private bool p2smart = false;
	
	//images for popup windows
	public GUIStyle instructionsPopup;
	public GUIStyle creditsPopup;
	public GUIStyle wipPopup;
	
	//used in determining button size & position
	private int buttonH;		//height of button
	private int buttonW;		//width of button
	private int buttonSpace;	//space between buttons
	private int buttonHoriz;	//x-coordinate of button
	private int buttonVert;		//y-coordinate of button
	
	// Almost everything GUI-related goes here
	void OnGUI() {
		//initializing...
		buttonH = 51;
		buttonW = 276;
		buttonSpace = 2;
		buttonHoriz = (int) Screen.width/2-buttonW/2;
		buttonVert = 100;
		rectPopup = new Rect((int)Screen.width/2-480,80,960,520);
		namePopup = new Rect((int)Screen.width/2-250,(int)Screen.height/2-100,500,200);
		GUI.skin = mySkin;
		
		//Game Logo
		GUI.Box(new Rect(0,10,Screen.width,93),logo);
		
		if(!showPopup){
			//Play button
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",play)) {
				windowID = 3;
				popupToggle();
			/**************
			Eventually offer options for play: Single Player, Multiplayer, Custom Game
			****************/
			}
	
			//High scores button
			updateButtonVert();
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",highscores)) {
				Application.LoadLevel("Scoreboard");
			}
			
			//Instructions button
			updateButtonVert();
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",instructions)) {
				windowID = 0;
				popupToggle();
			}
			
			//Ship Models button
			updateButtonVert();
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",ships)) {
				Application.LoadLevel("Ships");
			}
			
			//Credits button
			updateButtonVert();
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",credits)) {
				windowID = 1;
				popupToggle();
			}
	
			//Quit button
			updateButtonVert();
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",quit))
				Application.Quit();
		}
		else {
			if (windowID == 3)
				namePopup = GUI.Window(windowID, namePopup, PopupWindow, "");
			else
				rectPopup = GUI.Window(windowID, rectPopup, PopupWindow, "");
			GUI.FocusWindow(windowID);
		}
	}
	
	//Separate window that pops up
	private void PopupWindow(int windowID) {
		//Instructions
		if (windowID == 0) {
			GUI.Box(new Rect(0,0,960,520),"",instructionsPopup);
			if (GUI.Button(new Rect(850,460,92,37),"",back)) {
				popupToggle();
			}
		}
		
		//Credits
		else if (windowID == 1) {
			GUI.Box(new Rect(0,0,960,520),"",creditsPopup);
			if (GUI.Button(new Rect(850,460,92,37),"",back)) {
				popupToggle();
			}			
		}
		
		//Stuff to be updated (WIP)
		else if (windowID == 2) {
			GUI.Box(new Rect(0,0,960,520),"",wipPopup);
			if (GUI.Button(new Rect(850,460,92,37),"",back)) {
				popupToggle();
			}			
		}
		
		//Name prompt for high scores
		else if (windowID == 3) {
			GUI.Box(new Rect(0,0,500,25),"Enter Your Name");
			pName = GUI.TextField(new Rect(0,30,500,25), pName, 50);
			playerName = pName;
			
			//toggle options
			GUI.Box(new Rect(0,60,250,25), "Player 1 Option");
			GUI.Box(new Rect(250,60,250,25), "Player 2 Option");
			
			p1human = GUI.Toggle (new Rect(0,90,250,25), p1human, "Human");
			if (p1human) {
				p1random = false;
				p1smart = false;
			}
			p1random = GUI.Toggle (new Rect(0,120,250,25), p1random, "Random AI");
			if (p1random) {
				p1human = false;
				p1smart = false;
			}
			p1smart = GUI.Toggle(new Rect(0,150,250,25), p1smart, "Smart AI");
			if (p1smart) {
				p1human = false;
				p1random = false;
			}
			
			p2random = GUI.Toggle(new Rect(250,90,250,25), p2random, "Random AI");
			if (p2random)
				p2smart = false;
			p2smart = GUI.Toggle(new Rect(250,120,250,25), p2smart, "Smart AI");
			if (p2smart)
				p2random = false;
			
			//"First Player: Second Player"
				
			if (GUI.Button(new Rect(0,180,500,25),"Start Game")) {
				if (p1human && p2random)
					gameType = "Human:Random";
				else if (p1human && p2smart)
					gameType = "Human:Smart";
				else if (p1random && p2random)
					gameType = "Random:Random";
				else if (p1random && p2smart)
					gameType = "Random:Smart";
				else if (p1smart && p2random)
					gameType = "Smart:Random";
				else if (p1smart && p2smart)
					gameType = "Smart:Smart";
				Debug.Log(gameType);
				Application.LoadLevel("Game");
			}
		}
	}
				
	//toggles popup window on/off
	private void popupToggle() {
		if (showPopup == true)
			showPopup = false;
		else
			showPopup = true;
	}
	
	//creates new y-coordinate for next button
	private void updateButtonVert() {
		buttonVert += (buttonH+buttonSpace);
	}
}
