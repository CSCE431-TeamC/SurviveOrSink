using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battleship;

public class ShipPlacementGUI : MonoBehaviour
{

	int space;
	int ship;
	bool selected = true;
	bool vert = true;
	Vector3 screenPoint;
	Vector3 offset;
	Ray ray;
	RaycastHit hit;
	public static GameObject[,] gameGrid = new GameObject[10, 10];
	List<List<int>> shipPlacementTracker;
	
	// Use this for initialization
	void Start ()
	{
		whichShip (SmartAI.shipToPlace);
		
		Material emptyBoard = (Material)Resources.Load ("Empty", typeof(Material));
		
		//draw grids using individual cubes and store in dictionary (NxN)
		for (int rows = 0; rows < 10; ++rows) {
			for (int cols = 0; cols < 10; ++cols) {
				
				//gameGrid
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			
				cube.renderer.material = emptyBoard;
				
				cube.transform.position = new Vector3 (rows, (float)-1, cols);
				cube.transform.localScale += new Vector3 (0, (float)-.5, 0);
				gameGrid [rows, cols] = cube;
				
				if(Init_board.gameGrid[rows, cols].renderer.material.color == Color.green) {
					gameGrid [rows, cols].renderer.material.color = Color.green;	
				}
				
			}
		}
	}
	
	void OnGUI ()
	{
		
		if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 70, 200, 20), "Confirm Selection")) {
			
			for(int i = 0; i < gameGrid.GetLength(0); i++) {
				for(int j = 0; j < gameGrid.GetLength(1); j++) {
					if(gameGrid[i, j].renderer.material.color == Color.green) {
						Init_board.gameGrid[i,j].renderer.material.color = Color.green;
					}
				}
			}
			
			
			selected = false;
			SmartAI.waitingForRoutine = false;
			SmartAI.curship++;
			Application.LoadLevel (PlayerPrefs.GetInt ("previousLevel"));
			//Destroy (this.gameObject);
			//...	
		}
		
		if (GUI.Button (new Rect (Screen.width - 200, Screen.height - 100, 200, 20), "Clear Selection")) {
			selected = true;
			shipPlacementTracker.Clear ();
			resetMaterial (1);
			//...	
		}
		
		/*
		if (GUI.Button (new Rect (50, Screen.height - 120, 100, 20), "Aircraft Carrier")) {
			space = 5;
			selected = true;
			//...	
		}
		
		if (GUI.Button (new Rect (50, Screen.height - 100, 100, 20), "Battleship")) {
			space = 4;
			selected = true;
			//...	
		}
		
		if (GUI.Button (new Rect (50, Screen.height - 80, 100, 20), "Destroyer")) {
			space = 3;
			selected = true;
			//...	
		}
		
		if (GUI.Button (new Rect (50, Screen.height - 60, 100, 20), "Submarine")) {
			space = 3;
			selected = true;
			//...	
		}
		
		if (GUI.Button (new Rect (50, Screen.height - 40, 100, 20), "PT Boat")) {
			space = 2;
			selected = true;
			//...	
		}*/
		
	}
	
	public void whichShip (Ship ship)
	{
		if (ship != null) {
			switch (ship.getShipType ()) {
			
			case ShipType.AIRCRAFT_CARRIER:
				space = 5;
				break;
			
			case ShipType.BATTLESHIP:
				space = 4;
				break;
			
			case ShipType.DESTROYER:
				space = 3;
				break;
			
			case ShipType.SUBMARINE:
				space = 3;
				break;
			
			case ShipType.PATROL_BOAT:
				space = 2;
				break;
			
			default:
				space = -1;
				break;
		
			}
		}
	}
	
	bool IndicesForGameObject (GameObject obj, out int row, out int col)
	{
		
		bool found = false;
		
		int i = 0;
		int j = 0;
	
		//for (i = 0; i < Init_board.gameGrid.GetLength(0); i++) {
		//for (j = 0; j < Init_board.gameGrid.GetLength(1); j++) {

		for (i = 0; i < gameGrid.GetLength(0); i++) {
			for (j = 0; j < gameGrid.GetLength(1); j++) {
				
				//if (Init_board.gameGrid [i, j].Equals (obj)) {	
				if (gameGrid [i, j].Equals (obj)) {		
					found = true;
					row = i;
					col = j;
					return found;
				} else {
				}
			}
		}
		
		if (found) {
			row = i - 1;
			col = j - 1;
		} else {
			row = -1;
			col = -1;
		}
		
		return found;
		
	}
	
	void resetMaterial (int arg)
	{
	
		Material emptyBoard = (Material)Resources.Load ("Empty", typeof(Material));
		
		//for (int i = 0; i < Init_board.gameGrid.GetLength(0); i++) {
		//for (int j = 0; j < Init_board.gameGrid.GetLength(1); j++) {
				
		for (int i = 0; i < gameGrid.GetLength(0); i++) {
			for (int j = 0; j < gameGrid.GetLength(1); j++) {
				
				if (arg == 0) {
					//if (Init_board.gameGrid [i, j].renderer.material.color != Color.green) {	
					//Init_board.gameGrid [i, j].renderer.material = emptyBoard;
					if (gameGrid [i, j].renderer.material.color != Color.green) {	
						gameGrid [i, j].renderer.material = emptyBoard;
					
					} else {
						//nothing	
					}
				} else {
					//Init_board.gameGrid [i, j].renderer.material = emptyBoard;
					gameGrid [i, j].renderer.material = emptyBoard;
				}
			}
		}
	}
	
	void RayHit ()
	{	
		
		int row = 0;
		int column = 0;
		
		shipPlacementTracker = new List<List<int>> ();
		
		int diff;
		
		if (Physics.Raycast (ray, out hit, 200)) {
			if (hit.collider.gameObject.renderer) {
				
				resetMaterial (0);
				if (hit.collider.gameObject.renderer.material.color != Color.green) {
					hit.collider.gameObject.renderer.material.color = Color.red;
				
					IndicesForGameObject (hit.collider.gameObject, out row, out column);
				
					if (row >= 0 && column >= 0) {
				
						diff = (space / 2);
				
						for (int i = 0; i < space; i++) {
					
							int tempRow = row;
							int tempColumn = column;
					
							if (vert) {
						
								if (((row - diff) + i) < 0) {
									tempRow = 0;
								} else if (((row - diff) + i) > 9) {
									tempRow = 9;	
								} else {
									tempRow = ((row - diff) + i);	
								}
						
								tempColumn = column;
					
								//if (Init_board.gameGrid [tempRow, tempColumn].renderer.material.color != Color.green) {
								
								//Init_board.gameGrid [tempRow, tempColumn].renderer.material.color = Color.red;
									
								if (gameGrid [tempRow, tempColumn].renderer.material.color != Color.green) {
								
									gameGrid [tempRow, tempColumn].renderer.material.color = Color.red;
								
								}
									
								List<int> tempList = new List<int> ();
								tempList.Add (tempRow);
								tempList.Add (tempColumn);
								tempList.Add (ship);
								
								shipPlacementTracker.Add (tempList);
						
							} else {
						
								if (((column - diff) + i) < 0) {
									tempColumn = 0;
								} else if (((column - diff) + i) > 9) {
									tempColumn = 9;	
								} else {
									tempColumn = ((tempColumn - diff) + i);	
								}
						
								tempRow = row;
						
								//if (Init_board.gameGrid [tempRow, tempColumn].renderer.material.color != Color.green) {
								
								//Init_board.gameGrid [tempRow, tempColumn].renderer.material.color = Color.red;
								
								if (gameGrid [tempRow, tempColumn].renderer.material.color != Color.green) {
								
									gameGrid [tempRow, tempColumn].renderer.material.color = Color.red;
									
								}	
									
								List<int> tempList = new List<int> ();
								tempList.Add (tempRow);
								tempList.Add (tempColumn);
								tempList.Add (ship);
								
								shipPlacementTracker.Add (tempList);
								
							}
						}
					} else {
						//out of range check	
					}
				} else {
					//color check
				}
			} else {
				//hit collider
			}
		} else {
			//raycast ray
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		//ray = GameObject.Find("Left Camera").camera.ScreenPointToRay(Input.mousePosition);
		
		ray = camera.ScreenPointToRay (Input.mousePosition);
		
		if (Input.GetKeyDown ("r")) {
			vert = !vert;
		}
		
		if (selected) {
		
			RayHit ();
			if (Input.GetMouseButtonDown (0)) {
				
				for (int i = 0; i < shipPlacementTracker.Count; i++) {
					//Init_board.gameGrid[shipPlacementTracker [i] [0], shipPlacementTracker [i] [1]].renderer.material.color = Color.green;
					gameGrid [shipPlacementTracker [i] [0], shipPlacementTracker [i] [1]].renderer.material.color = Color.green;
				}
				
				selected = !selected;	
			}
			
		} else {
			space = 0;	
		}
		
	}
}
