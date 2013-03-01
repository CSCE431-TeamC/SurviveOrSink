using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Battleship;

public class RandomOpponent : ScriptableObject, IBattleshipOpponent
{
    public string Name { get { return "Random"; } }

    public static System.Random rand = new System.Random();
    Size gameSize;
	List<Point> shipSpots = new List<Point>();
	Material shotSquare = (Material)Resources.Load("Shot", typeof(Material));

    public void NewGame(Size size)
    {
		this.gameSize = size;
    }

    public void PlaceShips(ReadOnlyCollection<Ship> ships)
    {
		shipSpots.Clear();
        foreach (Ship s in ships)
        {
            s.Place(
                new Point(
                    rand.Next(this.gameSize.Width),
                    rand.Next(this.gameSize.Height)),
                (ShipOrientation)rand.Next(2));
			
			//save the location of our ships
			IEnumerable<Point> oneShipsLocations = s.GetAllLocations();
			foreach(Point p in oneShipsLocations)
			{
				shipSpots.Add(p);	
			}			
        }		
    }

    public Point GetShot()
    {
		var x = rand.Next(this.gameSize.Width);
		var y = rand.Next(this.gameSize.Height);
		//print(x + " " + y);
        return new Point(x,y);
		//return new Point(0,0);
		
    }

    public void NewMatch(string opponent) { }
    public void OpponentShot(Point shot) 
	{
		bool hit = false;
		//test to see if opponent's shot hit one of our ships or not
		foreach (Point p in shipSpots)
		{
			//hit one of our ship locations
			if (p == shot)
				hit = true;	
		}
		
		
		
		if(hit)
		{
			Init_board.gameGrid[shot.X, 9-shot.Y].renderer.material = shotSquare;
			Init_board.gameGrid[shot.X, 9-shot.Y].renderer.material.color = Color.red;
		}
		else
		{
			Init_board.gameGrid[shot.X, 9-shot.Y].renderer.material = shotSquare;
			Init_board.gameGrid[shot.X, 9-shot.Y].renderer.material.color = Color.white;
		}
	}
	
    public void ShotHit(Point shot, bool sunk) 
	{
		Init_board.observationGrid[shot.X, 9-shot.Y].renderer.material = shotSquare;
		Init_board.observationGrid[shot.X, 9-shot.Y].renderer.material.color = Color.red;
	}
	
    public void ShotMiss(Point shot) 
	{
		Init_board.observationGrid[shot.X, 9-shot.Y].renderer.material = shotSquare;
		Init_board.observationGrid[shot.X, 9-shot.Y].renderer.material.color = Color.white;	
	}
    public void GameWon() { }
    public void GameLost() { }
    public void MatchOver() { }
}
