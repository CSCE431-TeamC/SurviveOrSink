using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battleship;

public class Init_board : MonoBehaviour 
{
	public GUIStyle quit;
	public static GameObject[,] gameGrid = new GameObject[10,10];
	public static GameObject[,] observationGrid = new GameObject[10,10];
	public Vector2 scrollPosition = new Vector2(100,100);
	static string messages = "Begin Game!\nHi\nHello\n\nHello\n\n\n\nHello\n\n\nHello\n\n\n\nHit at A3\n\n\nThe End";
	
	void OnGUI()
	{
		var threeQuartersWidth = (int)(3 * Screen.width/4);
		if( (int) Time.timeSinceLevelLoad < 3)
		{
			GUI.Label(new Rect(threeQuartersWidth,25,100,100), "BEGIN GAME!");
		}
		
		if(GUI.Button(new Rect(Screen.width-100,0,100,20),"",quit))
			Application.Quit();
		GUILayout.BeginArea(new Rect((Screen.width/2)+125,20,Screen.width/2-125,(Screen.height/2)-20));
		//GUILayout.BeginArea(Rect((Screen.width/2)+125,20,Screen.width/2-125,(Screen.height/2)-20),"This is the text to be displayed");
		//GUILayout.BeginArea(new Rect (100,100,100,100));
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width/2-125), GUILayout.Height((Screen.height/2)-20));
		GUILayout.Label(messages);
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
	}
	
	// Use this for initialization
	void Start ()
	{
		Material emptyBoard = (Material)Resources.Load("Empty", typeof(Material));
		
		//draw grids using individual cubes and store in dictionary (NxN)
		for(int rows = 0; rows < 10; ++rows)
		{
			for(int cols = 0; cols < 10; ++cols)
			{
				
				//gameGrid
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cube.renderer.material = emptyBoard;
				/*
				if(rows % 2 == 0 && cols % 2 == 1)
					cube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else if(rows % 2 == 1 && cols % 2 == 0)
					cube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else
					cube.renderer.material.color = new Color((float).1, (float).1, (float).1);
					*/
				cube.transform.position = new Vector3(rows, (float)-1, cols);
				cube.transform.localScale += new Vector3(0, (float)-.5, 0);
				gameGrid[rows,cols] = cube;
				
				
				
				//observationGrid
				GameObject smallCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				/*
				if(rows % 2 == 0 && cols % 2 == 1)
					smallCube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else if(rows % 2 == 1 && cols % 2 == 0)
					smallCube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else
					smallCube.renderer.material.color = new Color((float).1, (float).1, (float).1);
					*/
				smallCube.renderer.material = emptyBoard;
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
        var op1 = go1.AddComponent<SmartAI>();

        var go2 = new GameObject("Player 2");
        var op2 = go2.AddComponent<RandomOpponent>();

        var battle = new GameObject("LocalBattle");
        battle.AddComponent<LocalBattle>().initialize(op1, op2, new Size(10, 10));
	
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
}
