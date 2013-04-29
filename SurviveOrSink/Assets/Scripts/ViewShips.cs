using UnityEngine;
using System.Collections;

public class ViewShips : MonoBehaviour {
	public GUISkin mySkin;
	public GUIStyle back;
	public GUIStyle background;
	private bool aircraft = true;
	private bool battleship = false;
	private bool destroyer = false;
	private bool patrol = false;
	private bool sub = false;
	void OnGUI() {
		GUI.skin = mySkin;
		GUI.Box(new Rect(0,0,200,190), "", background);
		GUI.Box(new Rect(0,10,200,30), "Choose a ship");
		
		if(GUI.Button(new Rect((int)Screen.width-102,10,102,47),"",back)) {
			Application.LoadLevel("MainMenu");
		}
		
		aircraft = GUI.Toggle(new Rect(10,40,100,25), aircraft, "Aircraft Carrier");
		if (aircraft) {
			battleship = false;
			destroyer = false;
			patrol = false;
			sub = false;
		}
		battleship = GUI.Toggle(new Rect(10,70,100,25), battleship, "Battleship");
		if (battleship) {
			aircraft = false;
			destroyer = false;
			patrol = false;
			sub = false;
		}
		destroyer = GUI.Toggle(new Rect(10,100,100,25), destroyer, "Destroyer");
		if (destroyer) {
			aircraft = false;
			battleship = false;
			patrol = false;
			sub = false;
		}
		patrol = GUI.Toggle(new Rect(10,130,100,25), patrol, "Patrol Boat");
		if (patrol) {
			aircraft = false;
			battleship = false;
			destroyer = false;
			sub = false;
		}
		sub = GUI.Toggle(new Rect(10,160,100,25), sub, "Submarine");
		if (sub) {
			aircraft = false;
			battleship = false;
			destroyer = false;
			patrol = false;
		}
		
		if (aircraft) {
			Vector3 newPos = new Vector3(0,2,-9);
			transform.position = newPos;
		}
		else if (battleship) {
			Vector3 newPos = new Vector3(7,2,-9);
			transform.position = newPos;
		}
		else if (destroyer) {
			Vector3 newPos = new Vector3(13,2,-9);
			transform.position = newPos;
		}
		else if (patrol) {
			Vector3 newPos = new Vector3(-6,2,-9);
			transform.position = newPos;
		}
		else if (sub) {
			Vector3 newPos = new Vector3(-13,2,-9);
			transform.position = newPos;
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
