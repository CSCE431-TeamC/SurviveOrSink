using System;
using System.Collections.ObjectModel;
using UnityEngine;
using Battleship;

public class HumanPlayer : ScriptableObject, IBattleshipOpponent 
{
    public string Name { get { return "Human"; } }
	
    Size gameSize;
	
    //initialize things here
    public void NewGame(Size size)
    {
		gameSize = size;
    }

    //prompt for user input for ship locations
    public void PlaceShips(ReadOnlyCollection<Ship> ships)
	{
		foreach (Ship s in ships)
        {
            s.Place(
                new Point(
                    RandomOpponent.rand.Next(this.gameSize.Width),
                    RandomOpponent.rand.Next(this.gameSize.Height)),
                (ShipOrientation)RandomOpponent.rand.Next(2),gameSize);
        }
    }

    //prompt user input for shot location
    public Point GetShot()
    {
        var x = RandomOpponent.rand.Next(this.gameSize.Width);
		var y = RandomOpponent.rand.Next(this.gameSize.Height);
        return new Point(x,y);
    }

    public void OpponentShot(Point shot) { }
    public void ShotHit(Point shot, bool sunk) { }
    public void ShotMiss(Point shot) { }
    public void GameWon() { }
    public void GameLost() { }


	// Use this for initialization
	void Start () 
	{
		

	}

	// Update is called once per frame
	void Update() 
	{
		/*
		if(transform.position.x < 5)
		{
			transform.Translate(1,0,0);
		}
		
		else
			transform.Translate(-5,0,0);*/
	}
}
