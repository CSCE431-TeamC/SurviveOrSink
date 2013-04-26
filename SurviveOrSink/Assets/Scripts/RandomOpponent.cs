using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Battleship;

public class RandomOpponent : BattleshipPlayer
{
    public static System.Random rand = new System.Random();
    Material shotSquare = (Material)Resources.Load("Shot", typeof(Material));
    Material pegMaterial = (Material)Resources.Load("Empty", typeof(Material));

    bool ready = false;

    int curship = 0;

    bool waitingForRoutine = false;

    public override void init(Size gamesize,bool player, params int[] shipSizes)
    {
        if (mShips == null) mShips = new List<Ship>();
        if (shipSizes == null || shipSizes.Length == 0)
        {
            foreach (ShipType type in Enum.GetValues(typeof(ShipType)))
            {
                switch (type)
                {
                    case ShipType.AIRCRAFT_CARRIER:
                        mShips.Add(new Ship_AircraftCarrier());
                        break;
                    case ShipType.BATTLESHIP:
                        mShips.Add(new Ship_Battleship());
                        break;
                    case ShipType.DESTROYER:
                        mShips.Add(new Ship_Destroyer());
                        break;
                    case ShipType.PATROL_BOAT:
                        mShips.Add(new Ship_PatrolBoat());
                        break;
                    case ShipType.SUBMARINE:
                        mShips.Add(new Ship_Submarine());
                        break;
                }
            }
        }
        else
        {
            foreach (int size in shipSizes)
            {
                mShips.Add(new Ship_Custom(size));
            }
        }


        this.gameSize = gamesize;
        this.ready = true;
        this.mPlayer = player;
    }

    void Update()
    {
        if (gameOver) return;
        if (!ready) return;
        if (!shipsReady)
        {
            if (!waitingForRoutine)
            {
                if (curship < mShips.Count)
                {
                    StartCoroutine(PlaceShip(mShips[curship]));
                    waitingForRoutine = true;
                }
                else
                {
                    curship = 0;
                    shipsReady = true;
                }
            }
        }
        else
        {
            if (mTurn && mTakeShot)
            {
                if (!waitingForRoutine && !shotReady)
                {
                    StartCoroutine(GetShot());
                    waitingForRoutine = true;
                }
            }
            else
            {
                if (!shipsSunk)
                {
					bool success = true;
                    foreach (Ship s in mShips)
                    {
						success &= s.IsSunk();
                    }
					shipsSunk = success;
                }
            }
        }
    }

    public System.Collections.IEnumerator PlaceShip(Ship ship)
    {
        yield return null;// return new WaitForSeconds(0.5f);

        if (ship != null)
        {
            int tries = 0;
            bool success = false;
            while (!success)
            {
                if (tries > 100) break; // no infinite loops

                if (!ship.Place(new Point(rand.Next(this.gameSize.Width), rand.Next(this.gameSize.Height)),
                            (ShipOrientation)rand.Next(2), gameSize))
                    continue;

                success = true;
                foreach (Ship s in mShips)
                {
                    if(s != ship)
                        success &= !ship.ConflictsWith(s);
                }

                tries++;
            }

            if (!success) throw new OperationCanceledException("PlaceShip");

            if (mPlayer)
            {
                /*foreach (Point p in ship.GetAllLocations())
                {
                    Init_board.gameGrid[p.X, p.Y].renderer.material.color = Color.green;
                }*/
                ship.LoadModel();
                ship.ShowModel();
            }
        }

        waitingForRoutine = false;
        curship++;
    }

    public System.Collections.IEnumerator GetShot()
    {
        yield return null;// return new WaitForSeconds(0.5f);

        bool success = false;
        while (!success)
        {
            shot = new Point(rand.Next(this.gameSize.Width), rand.Next(this.gameSize.Height));

            success = true;
            foreach (Point s in this.mShots)
            {
                if (s.X == shot.X && s.Y == shot.Y) success = false;
            }
        }

        waitingForRoutine = false;
        shotReady = true;
        this.mShots.Add(shot);
        //print("Shots: " + this.mShots.Count);
    }

    public override void OpponentHit(bool hit,bool sunk)
    {
        Material hitSquare = (Material)Resources.Load("hitSquare", typeof(Material));
        Material missSquare = (Material)Resources.Load("missSquare", typeof(Material));

        if (!mPlayer) return;
        if (hit)
        {
            Init_board.observationGrid[shot.X, shot.Y].renderer.material = hitSquare;
            Init_board.messages += "\nSuccessful! Hit a ship at: " + shot;
        }
        else
        {
            Init_board.observationGrid[shot.X, shot.Y].renderer.material = missSquare;
            Init_board.messages += "\nMissed! Shot at: " + shot;
        }
    }

    protected override void ShotHit(Point shot)
    {
        if (!mPlayer) return;
        ParticleSystem fire = ParticleSystem.Instantiate(Resources.Load("Fire1"), new Vector3(shot.X, -0.75f, shot.Y), Quaternion.identity) as ParticleSystem;
    }

    protected override void ShotMiss(Point shot)
    {
        if (!mPlayer) return;

        GameObject[] blocks = GameObject.FindGameObjectsWithTag("BoardPiece");
        foreach (GameObject block in blocks)
        {
            string[] split = block.name.Split(new Char[] { ',' });
            if (split.Length == 2)
            {
                if (Convert.ToInt32(split[0]) == shot.X && Convert.ToInt32(split[1]) == shot.Y)
                {
                    block.renderer.material.color = Color.black;
                    break;
                }
            }
        }

        ParticleSystem splash = ParticleSystem.Instantiate(Resources.Load("Splash"), new Vector3(shot.X, -1.0f, shot.Y), Quaternion.identity) as ParticleSystem;
    }
    public override void GameWon()
    {
        gameOver = true;
        gameWon = true;
    }
    public override void GameLost()
    {
        gameOver = true;
        gameWon = false;
    }
}
