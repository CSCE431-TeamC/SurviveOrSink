using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Battleship;

public class LocalBattle : MonoBehaviour
{

    private List<BattleshipPlayer> opponents;

    private int playerone = 0;
    private int playertwo = 1;

    private int currentplayer = 0;

    int _debug_framecount = 0;

    private Size boardSize;

    private List<int> shipSizes;        // if using a custom game type
    private bool useModels;             //    use default ships

    private bool gameOver = false;
    private bool gameStarted = false;

    public void initialize(BattleshipPlayer op1, BattleshipPlayer op2, Size boardSize, params int[] shipSizes)
    {
        if (op1 == null) throw new ArgumentNullException("op1");
        if (op2 == null) throw new ArgumentNullException("op2");

        if (boardSize == null)
        {
            boardSize = new Size(10, 10);
        }

        if (boardSize.Width <= 2 || boardSize.Height <= 2) throw new ArgumentOutOfRangeException("boardSize");

        if (shipSizes == null || shipSizes.Length < 1)
        {
            this.useModels = true;
            this.shipSizes = null;
        }
        else
        {
            if (shipSizes.Where(s => s <= 0).Any()) throw new ArgumentOutOfRangeException("shipSizes");

            if (shipSizes.Max() > boardSize.Width && shipSizes.Max() > boardSize.Height) throw new ArgumentOutOfRangeException("shipSizes");
            if (shipSizes.Length > boardSize.Width && shipSizes.Length > boardSize.Height) throw new ArgumentOutOfRangeException("shipSizes");

            this.shipSizes = new List<int>(shipSizes);
            this.useModels = false;
        }

        this.opponents = new List<BattleshipPlayer>();
        this.opponents.Add(op1);
        this.opponents.Add(op2);

        this.boardSize = boardSize;

        var rand = new System.Random();
        if (rand.Next(2) == 1)
        {
            currentplayer = playerone;
        }
        else
        {
            currentplayer = playertwo;
        }

        opponents[playerone].Score = 0;
        opponents[playertwo].Score = 0;

        if (useModels)
        {
            opponents[playerone].init(boardSize, true);
            opponents[playertwo].init(boardSize, false);
        }
        else
        {
            opponents[playerone].init(boardSize, true,shipSizes);
            opponents[playertwo].init(boardSize, false,shipSizes);
        }

        opponents[currentplayer].StartTurn();

        gameStarted = true;
    }

    void Update()
    {
        if (!gameStarted) return;
        if (gameOver) return;
        if (opponents[playerone].ShipsReady && opponents[playertwo].ShipsReady)
        {
            if (opponents[currentplayer].ShotReady) // has taken the shot
            {
                Point shot = opponents[currentplayer].Shot;
				Ship s = opponents[1 - currentplayer].SendShot(shot);
                if (s != null) // shot hits
                {
					opponents[currentplayer].OpponentHit(true,s.IsSunk());
                }
                else
                {
                    opponents[currentplayer].OpponentHit(false,false);
                }

                opponents[currentplayer].EndTurn();
                currentplayer = 1 - currentplayer;
                opponents[currentplayer].StartTurn();

                if (opponents[currentplayer].ShipsSunk) // game over
                {
                    print("Player " + currentplayer + " wins!");
                    opponents[currentplayer].GameWon();
                    opponents[1 - currentplayer].GameLost();
                    gameOver = true;
                }
            }
        }
    }
}

