namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class BattleshipCompetition
    {
        private IBattleshipOpponent op1;
        private IBattleshipOpponent op2;
        private Size boardSize;
        private List<int> shipSizes;

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
            this.shipSizes = new List<int>(shipSizes);
        }

        //public Dictionary<IBattleshipOpponent, int> RunCompetition()
        public IBattleshipOpponent RunCompetition()
        {
            var rand = new Random();

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
                ships[first] = (from s in this.shipSizes
                                select new Ship(s)).ToList();

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

            //place ships for player 2
            //calls player PlaceShips() function, will run until a valid placement occurs
            success = false;
            do
            {
                ships[second] = (from s in this.shipSizes
                                    select new Ship(s)).ToList();

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

                var ship = (from s in ships[1 - current]
                            where s.IsAt(shot)
                            select s).SingleOrDefault();

                if (ship != null)
                {
                    var sunk = ship.IsSunk(shots[current]);
                    opponents[current].ShotHit(shot, sunk);
                }
                else
                {
                    opponents[current].ShotMiss(shot);
                }

                var unsunk = (from s in ships[1 - current]
                                where !s.IsSunk(shots[current])
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
}