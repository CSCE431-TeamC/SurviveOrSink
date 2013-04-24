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
	
	//used in popup window
	private bool showPopup = false;	//toggles popup window for various submenus
	private Rect rectPopup;			//rectangle for popup window
	private int windowID = -1;
	
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
		GUI.skin = mySkin;
		
		//Game Logo
		GUI.Box(new Rect(0,10,Screen.width,93),logo);
		
		if(!showPopup){
			//Play button
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",play))
				Application.LoadLevel("Game");
			/**************
			Eventually offer options for play: Single Player, Multiplayer, Custom Game
			****************/
	
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
			
			//Options button
			updateButtonVert();
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",options)) {
				//Load options
				windowID = 2;
				popupToggle();
			}
			
			//Ship Models button
			updateButtonVert();
			if(GUI.Button(new Rect(buttonHoriz,buttonVert,buttonW,buttonH),"",ships)) {
				windowID = 2;
				popupToggle();
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
