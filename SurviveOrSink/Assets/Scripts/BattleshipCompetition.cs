using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Battleship;

public class BattleshipCompetition : MonoBehaviour
{
    private IBattleshipOpponent op1;
    private IBattleshipOpponent op2;
    private Size boardSize;
    private List<int> shipSizes;
	private List<ShipType> shipTypes; // for use with models
	private bool useModels;
	
	// for use with default game type - different ship models
    public BattleshipCompetition(IBattleshipOpponent op1, IBattleshipOpponent op2)
    {
        if (op1 == null)
        {
            throw new ArgumentNullException("op1");
        }

        if (op2 == null)
        {
            throw new ArgumentNullException("op2");
        }

        this.op1 = op1;
        this.op2 = op2;
        this.boardSize = new Size(10,10);
		this.useModels = true;
		this.shipSizes = new List<int>();
		this.shipTypes = new List<ShipType>();
		
		this.shipTypes.Add(ShipType.AIRCRAFT_CARRIER);
		this.shipTypes.Add(ShipType.BATTLESHIP);
		this.shipTypes.Add(ShipType.DESTROYER);
		this.shipTypes.Add(ShipType.SUBMARINE);
		this.shipTypes.Add(ShipType.PATROL_BOAT);
    }
	
    public BattleshipCompetition(IBattleshipOpponent op1, IBattleshipOpponent op2, Size boardSize, params int[] shipSizes)
    {
        if (op1 == null)
        {
            throw new ArgumentNullException("op1");
        }

        if (op2 == null)
        {
            throw new ArgumentNullException("op2");
        }

        if (boardSize.Width <= 2 || boardSize.Height <= 2)
        {
            throw new ArgumentOutOfRangeException("boardSize");
        }

        if (shipSizes == null || shipSizes.Length < 1)
        {
            throw new ArgumentNullException("shipSizes");
        }

        if (shipSizes.Where(s => s <= 0).Any())
        {
            throw new ArgumentOutOfRangeException("shipSizes");
        }

        if (shipSizes.Sum() >= (boardSize.Width * boardSize.Height))
        {
            throw new ArgumentOutOfRangeException("shipSizes");
        }

        this.op1 = op1;
        this.op2 = op2;
        this.boardSize = boardSize;
		this.shipTypes = new List<ShipType>();
        this.shipSizes = new List<int>(shipSizes);
		this.useModels = false;
    }
	
    public IBattleshipOpponent RunCompetition()
    {
        var rand = new System.Random();

        var opponents = new Dictionary<int, IBattleshipOpponent>();
        var scores = new Dictionary<int, int>();
        var ships = new Dictionary<int, List<Ship>>();
        var shots = new Dictionary<int, List<Point>>();

        var first = 0;
        var second = 1;

        opponents[first] = this.op1;
        opponents[second] = this.op2;
        scores[first] = 0;
        scores[second] = 0;
        shots[first] = new List<Point>();
        shots[second] = new List<Point>();

        if (rand.NextDouble() >= 0.5)
        {
            var swap = first;
            first = second;
            second = swap;
        }

        bool success;

        //choose who goes first
        if (rand.NextDouble() >= 0.5)
        {
		
            var swap = first;
            first = second;
            second = swap;
        }

        shots[first].Clear();
        shots[second].Clear();

        opponents[first].NewGame(this.boardSize);
        opponents[second].NewGame(this.boardSize);

        //place ships for player 1
        //calls player PlaceShips() function, will run until a valid placement occurs
        success = false;
        do
        {
			if(useModels) {
				ships[first] = (from s in this.shipTypes
								select new Ship(s)).ToList();
			} else {
	            ships[first] = (from s in this.shipSizes
	                            select new Ship(s)).ToList();
			}

            opponents[first].PlaceShips(ships[first].AsReadOnly());

            bool allPlacedValidly = true;
            for (int i = 0; i < ships[first].Count; i++)
            {
                if (!ships[first][i].IsPlaced || !ships[first][i].IsValid(this.boardSize))
                {
                    allPlacedValidly = false;
                    break;
                }
            }
                
            if (!allPlacedValidly)
            {
                continue;
            }

            bool noneConflict = true;
            for (int i = 0; i < ships[first].Count; i++)
            {
                for (int j = i + 1; j < ships[first].Count; j++)
                {
                    if (ships[first][i].ConflictsWith(ships[first][j]))
                    {
                        noneConflict = false;
                        break;
                    }
                }

                if (!noneConflict)
                {
                    break;
                }
            }

            if (!noneConflict)
            {
                continue;
            }

            else
            {
                success = true;
            }
        } while (!success);
	
	/*
		IEnumerable<Point> temp = ships[first][0].GetAllLocations();
		foreach (Point p in temp)
			print (p.X + " " + p.Y);
		*/
	
        //place ships for player 2
        //calls player PlaceShips() function, will run until a valid placement occurs
        success = false;
        do
        {
			if(useModels) {
				ships[second] = (from s in this.shipTypes
									select new Ship(s)).ToList();
			} else {
	            ships[second] = (from s in this.shipSizes
	                                select new Ship(s)).ToList();
			}

            opponents[second].PlaceShips(ships[second].AsReadOnly());

            bool allPlacedValidly = true;
            for (int i = 0; i < ships[second].Count; i++)
            {
                if (!ships[second][i].IsPlaced || !ships[second][i].IsValid(this.boardSize))
                {
                    allPlacedValidly = false;
                    break;
                }
            }

            if (!allPlacedValidly)
            {
                continue;
            }

            bool noneConflict = true;
            for (int i = 0; i < ships[second].Count; i++)
            {
                for (int j = i + 1; j < ships[second].Count; j++)
                {
                    if (ships[second][i].ConflictsWith(ships[second][j]))
                    {
                        noneConflict = false;
                        break;
                    }
                }

                if (!noneConflict)
                {
                    break;
                }
            }

            if (!noneConflict)
            {
                continue;
            }
            else
            {
                success = true;
            }
        } while (!success);

        //game loop - play until all ships are sunk, alternating players
        var current = first;
        var winner = 0;
        while (true)
        {
            Point shot = opponents[current].GetShot();

            if (shots[current].Where(s => s.X == shot.X && s.Y == shot.Y).Any())
            {
                continue;
            }

            shots[current].Add(shot);

            opponents[1 - current].OpponentShot(shot);

            /*var ship = (from s in ships[1 - current]
                        where s.IsAt(shot)
                        select s).SingleOrDefault();*/
			
			foreach(var ship in ships[1-current]) {
				if(ship.testHit(shot)) {
					print ("Player " + (current+1) + " hit: (" + shot.X + "," + shot.Y + ")");
					var sunk = ship.IsSunk();
					if(sunk) print ("Player " + (current+1) + " sunk " + ship.Length);
					opponents[current].ShotHit(shot,sunk);
					break;
				} else {
					opponents[current].ShotMiss(shot);
				}
			}

            /*if (ship != null)
            {
                var sunk = ship.IsSunk();
                opponents[current].ShotHit(shot, sunk);
            }
            else
            {
                opponents[current].ShotMiss(shot);
            }*/

            var unsunk = (from s in ships[1 - current]
                            where !s.IsSunk()
                            select s);

            //current player has sunked all ships, break out of game loop
            if (!unsunk.Any()) 
            { 
                winner = current;
                var loser = 1 - current;
                opponents[winner].GameWon();
                opponents[loser].GameLost();
                break; 
            }

            current = 1 - current;
        }

        return opponents[winner];
    }

}
