using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Init_board : MonoBehaviour {
	
	List<GameObject> gameGrid = new List<GameObject>();
	List<GameObject> observationGrid = new List<GameObject>();
	
	// Use this for initialization
	void Start () 
	{
		//draw grids using individual cubes and store in dictionary (NxN)
		for(int rows = 0; rows < 10; ++rows)
		{
			for(int cols = 0; cols < 10; ++cols)
			{
				
				//gameGrid
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				if(rows % 2 == 0 && cols % 2 == 1)
					cube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else if(rows % 2 == 1 && cols % 2 == 0)
					cube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else
					cube.renderer.material.color = new Color((float).1, (float).1, (float).1);;
				cube.transform.position = new Vector3(rows-3, (float)-1, cols);
				cube.transform.localScale += new Vector3(0, (float)-.5, 0);
				gameGrid.Add(cube);
				
				
				
				//observationGrid
				GameObject smallCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				if(rows % 2 == 0 && cols % 2 == 1)
					smallCube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else if(rows % 2 == 1 && cols % 2 == 0)
					smallCube.renderer.material.color = new Color((float).25, (float).25, (float).25);
				else
					smallCube.renderer.material.color = new Color((float).1, (float).1, (float).1);
				double temp = rows+20-rows*.1;
				float x_loc = (float)temp;
				float y_loc = (float)(cols - cols*.1);
				smallCube.transform.position = new Vector3(x_loc, y_loc, 5);
				smallCube.transform.localScale += new Vector3((float)-.1, (float)-.1, (float)-.1);
				observationGrid.Add(smallCube);
				
			}
		}
		
		//gameGrid[16].renderer.material.color = Color.white;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
