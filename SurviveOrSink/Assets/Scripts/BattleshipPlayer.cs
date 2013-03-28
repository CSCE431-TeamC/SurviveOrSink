using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using Battleship;

public abstract class BattleshipPlayer : MonoBehaviour
{
    public string Name { get { return "Random"; } }

    int score = 0;
    protected List<Ship> mShips = new List<Ship>();
    protected List<Point> mShots = new List<Point>();	

    protected Size gameSize;
    protected bool gameOver;
    protected bool gameWon;

    protected bool shipsReady = false;
    protected bool shipsSunk = false;

    protected bool mTurn = false;
    protected bool mPlayer = false;
    protected bool mTakeShot = false;

    protected bool shotReady = false;
    protected Point shot;

    public bool ShipsReady
    {
        get
        {
            return this.shipsReady;
        }
    }

    public bool ShotReady
    {
        get
        {
            return this.shotReady;
        }
    }

    public bool ShipsSunk
    {
        get
        {
            return this.shipsSunk;
        }
    }

    public bool MyTurn
    {
        get
        {
            return this.mTurn;
        }
    }

    public int Score
    {
        set
        {
            this.score = value;
        }
        get
        {
            return this.score;
        }
    }

    public Point Shot
    {
        get
        {
            return this.shot;
        }
    }

    public void StartTurn()
    {
        this.shotReady = false;
        this.mTurn = true;
        this.mTakeShot = true;
    }

    public void EndTurn()
    {
        this.shotReady = false;
        this.mTurn = false;
        this.mTakeShot = false;
    }

    // player = whether to show hits on screen
    abstract public void init(Size boardsize,bool player,params int[] shipsizes);

    public Ship SendShot(Point p)
    {
        foreach (Ship s in this.mShips)
        {
            if (s.testHit(p))
            {
                this.ShotHit(p);
                return s;
            }
            else
            {
                this.ShotMiss(p);
            }
        }

        return null;
    }

    abstract public void OpponentHit(bool hit,bool sunk);

    abstract protected void ShotHit(Point shot);

    abstract protected void ShotMiss(Point shot);

    abstract public void GameWon();

    abstract public void GameLost();
}
