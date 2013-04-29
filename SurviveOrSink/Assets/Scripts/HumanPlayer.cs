using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Battleship;

public class HumanPlayer : BattleshipPlayer
{
    public static System.Random rand = new System.Random();
    Material pegMaterial = (Material)Resources.Load("Empty", typeof(Material));

    Ray mRay;
    Ray eRay;
    Point curRayCast = new Point();
    bool mHighlighted = false;
    bool eHighlighted = false;

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
        eRay = GameObject.Find("Right Camera").camera.ScreenPointToRay(Input.mousePosition);

        if (mHighlighted)
        {
            GameObject[] gridblocks = GameObject.FindGameObjectsWithTag("BoardPiece");
            foreach (GameObject block in gridblocks)
            {
                block.renderer.material.color = Color.white;
            }
            mHighlighted = false;
        }
        if (eHighlighted)
        {
            GameObject[] gridblocks = GameObject.FindGameObjectsWithTag("EnemyBoardPiece");
            foreach (GameObject block in gridblocks)
            {
                block.renderer.material.color = Color.white;
            }
            eHighlighted = false;
        }

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

                Point cast = getBoardRayCast(false, Color.white);

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

    public Point getBoardRayCast(bool highlight, Color col)
    {
        Point cast = new Point(-1, -1);

        RaycastHit hit;

        if (Physics.Raycast(mRay, out hit, 200, 1 << LayerMask.NameToLayer("BoardPiece")))
        {
            Renderer renderer = hit.collider.renderer;

            if (renderer)
            {
                if (highlight)
                {
                    renderer.material.color = col;
                    mHighlighted = true;
                }
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

    public Point getEnemyBoardRayCast(bool highlight, Color col)
    {
        Point cast = new Point(-1, -1);

        RaycastHit hit;

        if (Physics.Raycast(eRay, out hit, 200, 1 << LayerMask.NameToLayer("EnemyBoardPiece")))
        {
            Renderer renderer = hit.collider.renderer;

            if (renderer)
            {
                if (highlight)
                {
                    renderer.material.color = col;
                    eHighlighted = true;
                }
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
        bool success = false;
        while (!success)
        {
            yield return null;

            Point cast = getEnemyBoardRayCast(true, Color.green);

            if (Input.GetMouseButtonDown(0))
            {
                shot = cast;

                if (shot.X >= 0 && shot.X < 10 && shot.Y >= 0 && shot.Y < 10)
                {
                    success = true;
                    foreach (Point s in this.mShots)
                    {
                        if (s.X == shot.X && s.Y == shot.Y) success = false;
                    }
                }
            }
        }

        waitingForRoutine = false;
        shotReady = true;
        this.mShots.Add(shot);
        //print("Shots: " + this.mShots.Count);
    }

    public override void OpponentHit(bool hit, bool sunk)
    {
        Material hitSquare = (Material)Resources.Load("hitSquare", typeof(Material));
        Material missSquare = (Material)Resources.Load("missSquare", typeof(Material));

        if (!mPlayer) return;
        if (hit)
        {
            Init_board.observationGrid[shot.X, shot.Y].renderer.material = hitSquare;
            Init_board.messages += "\nSuccessful! Hit a ship at: " + shot;
			Init_board.score += 10;
			Sounds.playHit = true;
        }
        else
        {
            Init_board.observationGrid[shot.X, shot.Y].renderer.material = missSquare;
            Init_board.messages += "\nMissed! Shot at: " + shot;
			Init_board.score--;
			Sounds.playMiss = true;
        }
    }

    protected override void ShotHit(Point shot)
    {
        if (!mPlayer) return;
        ParticleSystem fire = ParticleSystem.Instantiate(Resources.Load("Fire1"),new Vector3(shot.X,-0.75f,shot.Y),Quaternion.identity) as ParticleSystem;
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
