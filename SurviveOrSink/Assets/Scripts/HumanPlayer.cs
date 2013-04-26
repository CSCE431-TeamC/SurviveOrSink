using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Battleship;

public class HumanPlayer : BattleshipPlayer
{
    public static System.Random rand = new System.Random();
    Material shotSquare = (Material)Resources.Load("Shot", typeof(Material));
    Material pegMaterial = (Material)Resources.Load("Empty", typeof(Material));

    Ray mRay;
    Point curRayCast = new Point();

    ShipOrientation placeOrientation = ShipOrientation.Horizontal;

    bool ready = false;

    int curship = 0;

    bool waitingForRoutine = false;

    public override void init(Size gamesize, bool player, params int[] shipSizes)
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

        mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

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
        bool placed = false;
        bool valid = false;

        if (ship != null)
        {
            ship.LoadModel();
            ship.HideModel();

            while (!placed)
            {
                yield return null;

                if (Input.GetKeyDown("r"))
                {
                    if(placeOrientation == ShipOrientation.Horizontal)
                        placeOrientation = ShipOrientation.Vertical;
                    else placeOrientation = ShipOrientation.Horizontal;
                }

                if (Input.GetMouseButtonDown(0) && valid)
                {
                    placed = true;
                    break;
                }

                Point cast = getBoardRayCast();
                Debug.Log("" + cast.X + "," + cast.Y);

                if (ship.Place(cast, placeOrientation, gameSize))
                {
                    bool collides = false;
                    foreach (Ship s in mShips)
                    {
                        if (s != ship)
                            collides |= ship.ConflictsWith(s);
                    }

                    if (!collides)
                    {
                        ship.ShowModel();
                        valid = true;
                    }
                    else
                    {
                        ship.HideModel();
                        valid = false;
                    }
                }
                else
                {
                    ship.HideModel();
                    valid = false;
                }
            }
        }

        waitingForRoutine = false;
        curship++;
    }

    public Point getBoardRayCast()
    {
        Point cast = new Point(-1,-1);

        RaycastHit[] hits = Physics.RaycastAll(mRay,200,1 << LayerMask.NameToLayer("BoardPiece"));

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.renderer;

            if (renderer)
            {
                string[] split = hit.collider.name.Split(new Char[] { ',' });
                if (split.Length == 2)
                {
                    cast.X = Convert.ToInt32(split[0]);
                    cast.Y = Convert.ToInt32(split[1]);
                }
            }
        }

        return cast;
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

    public override void OpponentHit(bool hit, bool sunk)
    {
        if (!mPlayer) return;
        if (hit)
        {
            Init_board.observationGrid[shot.X, shot.Y].renderer.material = shotSquare;
            Init_board.observationGrid[shot.X, shot.Y].renderer.material.color = Color.red;
            Init_board.messages += "\nSuccessful! Hit a ship at: " + shot;
        }
        else
        {
            Init_board.observationGrid[shot.X, shot.Y].renderer.material = shotSquare;
            Init_board.observationGrid[shot.X, shot.Y].renderer.material.color = Color.white;
            Init_board.messages += "\nMissed! Shot at: " + shot;
        }
    }

    protected override void ShotHit(Point shot)
    {
        if (!mPlayer) return;
        GameObject peg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        peg.renderer.material = pegMaterial;

        peg.renderer.material.color = Color.red;
        peg.transform.position = new Vector3(shot.X, -peg.transform.localScale.y * 0.6f, shot.Y);
        peg.transform.localScale -= new Vector3(0.3f, 0.4f, 0.3f);
        //Init_board.messages = Init_board.messages+="\n Opponent hit a ship at: "+shot;//Smart AI
    }

    protected override void ShotMiss(Point shot)
    {
        if (!mPlayer) return;
        GameObject peg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        peg.renderer.material = pegMaterial;

        peg.renderer.material.color = Color.white;
        peg.transform.position = new Vector3(shot.X, -peg.transform.localScale.y * 0.3f, shot.Y);
        peg.transform.localScale -= new Vector3(0.5f, 0.7f, 0.5f);
        //Init_board.messages = Init_board.messages+="\n Opponent missed shot at: "+shot;//Smart AI
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
