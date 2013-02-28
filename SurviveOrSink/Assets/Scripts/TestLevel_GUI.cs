using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Battleship;

public class TestLevel_GUI : MonoBehaviour {
	
	void OnGUI() {
		GUI.Box(new Rect(Screen.width-300,0,300,200),"Chat Window");
	}
	
	// Use this for initialization
	void Start () {
		
		var op1 = new SmartAI();
        var op2 = new RandomOpponent();

            BattleshipCompetition bc = new BattleshipCompetition(
                op1,
                op2,
                new Size(10, 10),       // Board Size
                2, 3, 3, 4, 5           // Ship Sizes
            );

            var winner = bc.RunCompetition();

            Console.WriteLine("{0} won the match!", winner.Name);
			print (winner.Name + " won the match!");
            //Console.ReadKey(true);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

