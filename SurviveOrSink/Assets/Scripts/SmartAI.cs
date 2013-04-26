using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Battleship;

public class SmartAI : BattleshipPlayer
{
    // UNKNOWN - we know nothing about this location
    // MISS    - an empty space
    // HIT     - a ship is here
    // SUNK    - a ship is here, and we've sunk it, but all we know is this location is SUNK
    // SUNKSHIP - we've sunk the ship here and we know where the rest of the ship is
    private enum State {UNKNOWN = 0, MISS, HIT, SUNK, SUNKSHIP};
    private char[] stateChar = { '.', '-', 'x', '#', '^' };

    // what we know about the world:
    private int width, height;
    private State[,] state;
    // the size of the remaining ships that we haven't sunk
    private List<int> remainingShips;
    // an optional list of points that we consider first when deciding where to explore
    private List<Point> mustExplore;
    // the amount of open space around each (x,y)
    private int[,] left, right, below, above;
    // metric describing how much space/possible hits each (x,y) has
    private int[,] space, hits;

    private bool DEBUG = false;
    private System.Random rand = new System.Random();

    //public string Name { get { return "Smart AI"; } }
	
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
		
		width = gamesize.Width;
        height = gamesize.Height;
        state = new State[width, height];
        left = new int[width, height];
        right = new int[width, height];
        below = new int[width, height];
        above = new int[width, height];
        space = new int[width, height];
        hits = new int[width, height];
        remainingShips = new List<int>();
        mustExplore = new List<Point>();
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

	/*
    public void NewGame(Size size)
    {
        width = size.Width;
        height = size.Height;
        state = new State[width, height];
        left = new int[width, height];
        right = new int[width, height];
        below = new int[width, height];
        above = new int[width, height];
        space = new int[width, height];
        hits = new int[width, height];
        remainingShips = new List<int>();
        mustExplore = new List<Point>();
    }

    // nothing fancy, just random placement.  Also note which ships we have available.
    public void PlaceShips(ReadOnlyCollection<Ship> ships)
    {
        remainingShips.Clear();
        foreach (Ship s in ships)
        {
            remainingShips.Add(s.Length);
            s.Place(
                new Point(
                    rand.Next(width),
                    rand.Next(height)),
                (ShipOrientation)rand.Next(2),new Size(10,10));
        }
    }*/
	
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
				
				remainingShips.Add(ship.Length);

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

    // where should we shoot?
    public System.Collections.IEnumerator GetShot()
    {
		yield return null;// return new WaitForSeconds(0.5f);
		
        // update some state:
        // convert HITs that are surrounded into SUNKs
        SunkFromSurroundedHits();
        // look for any HIT/SUNKs that could only belong to one ship
        SunkFromShipConstraints();
		
		bool success = false;
		while(!success)
		{
        	// if we're certain that a ship must be at some point, choose that
	        if (mustExplore.Count > 0)
	        {
	            Point p = mustExplore[0];
	            mustExplore.RemoveAt(0);
	            shot = p;
	        }
	        else
	        {
	            // check for any outstanding HITs that need to be explored
	            var hitPoint = GetHitPoint();
	            if (hitPoint != null)
				{
					shot = (Point)hitPoint;
				}
	            // if there are no outstanding hits, explore empty space
	            else shot = GetSpacePoint();
	        }
			
			success = true;
            foreach (Point s in this.mShots)
            {
                if (s.X == shot.X && s.Y == shot.Y) success = false;
            }
		}
		
		waitingForRoutine = false;
        shotReady = true;
        this.mShots.Add(shot);
    }
	
	//when we fire at opponent, update board based on what happens
	public override void OpponentHit(bool hit, bool sunk)
    {
        Material hitSquare = (Material)Resources.Load("hitSquare", typeof(Material));
        Material missSquare = (Material)Resources.Load("missSquare", typeof(Material));

		if (!mPlayer) return;
		Point shot = mShots[mShots.Count-1];

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
		
		int x = shot.X, y = shot.Y;
        
		if(hit)
		{
			state[x, y] = State.HIT;
		}
		else if (!hit)
		{
			state[x, y] = State.MISS;	
		}
		
		if(sunk)
		{
			state[x, y] = State.SUNK;
		}		

        // if we know that we just sunk a ship at (x,y), look at adjacent locations to see if we
        // had to have sunk a given ship (i.e. if there's only one remaining ship that has a size
        // less than the sunk length, that ship must have sunk.)
        if (sunk)
        {
            int left = x;
            while (left > 0 && (state[left - 1, y] == State.HIT || state[left - 1, y] == State.SUNK)) --left;
            int right = x;
            while (right < width - 1 && (state[right + 1, y] == State.HIT || state[right + 1, y] == State.SUNK)) ++right;
            int above = y;
            while (above > 0 && (state[x, above - 1] == State.HIT || state[x, above - 1] == State.SUNK)) --above;
            int below = y;
            while (below < height - 1 && (state[x, below + 1] == State.HIT || state[x, below + 1] == State.SUNK)) ++below;

            int sunkWidth = right - left + 1;
            int sunkHeight = below - above + 1;

            if (sunkWidth > 1 && sunkHeight > 1) return;
            else if (sunkWidth > 1)
            {
                int numShips = 0;
                foreach (int w in remainingShips) if (w <= sunkWidth) ++numShips;
                if (numShips == 1) remainingShips.Remove(sunkWidth);
            }
            else if (sunkHeight > 1)
            {
                int numShips = 0;
                foreach (int w in remainingShips) if (w <= sunkHeight) ++numShips;
                if (numShips == 1) remainingShips.Remove(sunkHeight);
            }
        }

        if (DEBUG) Console.WriteLine("{0} State.HIT {1}", shot, sunk);
    }

    // when we are hit, update gui
    protected override void ShotHit(Point shot)
    {
		if (!mPlayer) return;
        GameObject peg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        peg.renderer.material = pegMaterial;

        peg.renderer.material.color = Color.red;
        peg.transform.position = new Vector3(shot.X, -peg.transform.localScale.y * 0.6f, shot.Y);
        peg.transform.localScale -= new Vector3(0.3f, 0.4f, 0.3f);
		//Init_board.messages = Init_board.messages+="\n Opponent hit a ship at: "+shot;//opponent is Random AI
    }

    // when we are shot at but missed, update gui
    protected override void ShotMiss(Point shot)
    {
		if (!mPlayer) return;
        GameObject peg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        peg.renderer.material = pegMaterial;

        peg.renderer.material.color = Color.white;
        peg.transform.position = new Vector3(shot.X, -peg.transform.localScale.y * 0.3f, shot.Y);
        peg.transform.localScale -= new Vector3(0.5f, 0.7f, 0.5f);
		
        if (DEBUG) Console.WriteLine("{0} miss", shot);
		//Init_board.messages = Init_board.messages+="\n Opponent missed and shot at: "+shot;//opponent is Random AI
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
	
    // helper functions

    // examine each HIT/SUNK, checking how the remaining ships could fit on it.  If there is only
    // one ship that could fit on this space in one way, then remove that ship from consideration
    // and mark the associated locations as SUNKSHIP.
    private void SunkFromShipConstraints()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (state[x, y] == State.HIT || state[x,y] == State.SUNK)
                {
                    int left = x;
                    while (left > 0 && (state[left - 1, y] == State.UNKNOWN || state[left - 1, y] == State.HIT || state[left - 1, y] == State.SUNK)) --left;
                    int right = x;
                    while (right < width - 1 && (state[right + 1, y] == State.UNKNOWN || state[right + 1, y] == State.HIT || state[right + 1, y] == State.SUNK)) ++right;
                    int above = y;
                    while (above > 0 && (state[x, above - 1] == State.UNKNOWN || state[x, above - 1] == State.HIT || state[x, above - 1] == State.SUNK)) --above;
                    int below = y;
                    while (below < height - 1 && (state[x, below + 1] == State.UNKNOWN || state[x, below + 1] == State.HIT || state[x, below + 1] == State.SUNK)) ++below;

                    int sunkWidth = right - left + 1;
                    int sunkHeight = below - above + 1;

                    if (sunkWidth > 1 && sunkHeight > 1) continue;
                    else if (sunkWidth > 1)
                    {
                        int numShips = 0, lastW = 0;
                        foreach (int w in remainingShips)
                        {
                            if (w <= sunkWidth)
                            {
                                lastW = w;
                                ++numShips;
                            }
                        }
                        if (numShips == 1 && lastW == sunkWidth)
                        {
                            for (int i = left; i <= right; ++i)
                            {
                                if (state[i, y] == State.UNKNOWN) mustExplore.Add(new Point(i, y));
                                state[i, y] = State.SUNKSHIP;
                            }
                            remainingShips.Remove(sunkWidth);
                        }
                    }
                    else if (sunkHeight > 1)
                    {
                        int numShips = 0, lastH = 0;
                        foreach (int w in remainingShips)
                        {
                            if (w <= sunkHeight)
                            {
                                lastH = w;
                                ++numShips;
                            }
                        }
                        if (numShips == 1 && lastH == sunkHeight)
                        {
                            for (int i = above; i <= below; ++i)
                            {
                                if (state[x, i] == State.UNKNOWN) mustExplore.Add(new Point(x, i));
                                state[x, i] = State.SUNKSHIP;
                            }
                            remainingShips.Remove(sunkHeight);
                        }
                    }
                }
            }
        }
    }

    // check each HIT to see if it is completely surrounded by zero or more HITs followed by 
    // an edge or other termination piece.  If so, mark it as SUNK.
    private void SunkFromSurroundedHits()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (state[x, y] == State.HIT)
                {
                    int left = x;
                    while (left > 0 && state[left - 1, y] == State.HIT) --left;
                    if (left > 0 && state[left - 1, y] == State.UNKNOWN) continue; // not bounded
                    int right = x;
                    while (right < width - 1 && state[right + 1, y] == State.HIT) ++right;
                    if (right < width - 1 && state[right + 1, y] == State.UNKNOWN) continue;
                    int above = y;
                    while (above > 0 && state[x, above - 1] == State.HIT) --above;
                    if (above > 0 && state[x, above - 1] == State.UNKNOWN) continue;
                    int below = y;
                    while (below < height - 1 && state[x, below + 1] == State.HIT) ++below;
                    if (below < height - 1 && state[x, below + 1] == State.UNKNOWN) continue;

                    // if we get here, we know that this State.HIT square is bounded on all sides by zero
                    // or more State.HITs and then a non-State.UNKNOWN piece or edge.  Since it's surrounded, it's sunk.
                    state[x, y] = State.SUNK;
                }
            }
        }
    }

    // for each HIT, examine all the configurations of remaining ships that could fit on that location
    private Point? GetHitPoint()
    {
        int hitSum = 0;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                hits[x, y] = 0;
            }
        }

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (state[x, y] == State.HIT)
                {
                    foreach (int w in remainingShips)
                    {
                        // horizontal
                        int left = x, endl = Math.Max(x - (w - 1), 0);
                        while (left > endl && (state[left - 1, y] == State.UNKNOWN || state[left - 1, y] == State.HIT || state[left - 1, y] == State.SUNK)) --left;
                        int right = x, endr = Math.Min(x + (w - 1), width - 1);
                        while (right < endr && (state[right + 1, y] == State.UNKNOWN || state[right + 1, y] == State.HIT || state[right + 1, y] == State.SUNK)) ++right;
                        if (right - left + 1 >= w)
                        {
                            for (int i = left; i <= right; ++i)
                            {
                                if (state[i, y] == State.UNKNOWN)
                                {
                                    ++hits[i, y];
                                    ++hitSum;
                                }
                            }
                        }
                        // vertical
                        int above = y, enda = Math.Max(y - (w - 1), 0);
                        while (above > enda && (state[x, above - 1] == State.UNKNOWN || state[x, above - 1] == State.HIT || state[x, above - 1] == State.SUNK)) --above;
                        int below = y, endb = Math.Min(y + (w - 1), height - 1);
                        while (below < endb && (state[x, below + 1] == State.UNKNOWN || state[x, below + 1] == State.HIT || state[x, below + 1] == State.SUNK)) ++below;
                        if (below - above + 1 >= w)
                        {
                            for (int i = above; i <= below; ++i)
                            {
                                if (state[x, i] == State.UNKNOWN)
                                {
                                    ++hits[x, i];
                                    ++hitSum;
                                }
                            }
                        }
                    }
                }
            }
        }
        // if we haven't marked any location as possible matches for existing HITs, do something else
        if (hitSum == 0) return null;
        if (DEBUG) displayHitState();
        // choose randomly among the best hit locations
        return GetLargest(hits);
    }

    // examine how many different ways the remaining ships could fit in the open space surrounding each position
    private Point GetSpacePoint()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                ComputeAdjacentSpace(x, y);
                int sumPoss = 0;
                foreach (int len in remainingShips)
                {
                    // left/right
                    int leftPos = left[x, y] < len - 1 ? left[x, y] : len - 1;
                    int rightPos = right[x, y] < len - 1 ? right[x, y] : len - 1;
                    int possLR = leftPos + rightPos + 1 - len + 1;
                    if (possLR > 0) sumPoss += possLR;
                    // above/below
                    int abovePos = above[x, y] < len - 1 ? above[x, y] : len - 1;
                    int belowPos = below[x, y] < len - 1 ? below[x, y] : len - 1;
                    int possAB = abovePos + belowPos + 1 - len + 1;
                    if (possAB > 0) sumPoss += possAB;
                }
                space[x, y] = sumPoss;
            }
        }
        if (DEBUG) displaySpaceState();

        // choose randomly among the best space locations
        return GetLargest(space);
    }

    // compute the amount of open space in each direction from (x,y)
    private void ComputeAdjacentSpace(int x, int y)
    {
        if (state[x, y] != State.UNKNOWN) left[x, y] = right[x, y] = below[x, y] = above[x, y] = 0;
        else
        {
            // left
            if (x == 0 || state[x - 1, y] != State.UNKNOWN) left[x, y] = 0;
            else left[x, y] = left[x - 1, y] + 1;
            // right
            if (x == 0 || right[x - 1, y] == 0)
            {
                int ctr = 0;
                for (int xp = x + 1; xp < width; ++xp)
                {
                    if (state[xp, y] != State.UNKNOWN) break;
                    ++ctr;
                }
                right[x, y] = ctr;
            }
            else right[x, y] = right[x - 1, y] - 1;
            // above
            if (y == 0 || state[x, y - 1] != State.UNKNOWN) above[x, y] = 0;
            else above[x, y] = above[x, y - 1] + 1;
            // below
            if (y == 0 || below[x, y - 1] == 0)
            {
                int ctr = 0;
                for (int yp = y + 1; yp < height; ++yp)
                {
                    if (state[x, yp] != State.UNKNOWN) break;
                    ++ctr;
                }
                below[x, y] = ctr;
            }
            else below[x, y] = below[x, y - 1] - 1;
        }
    }

    // find the largest element(s) in arr and choose one at random
    private Point GetLargest(int[,] arr)
    {
        // find largest element in arr
        int largest = 0, count = 0;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (arr[x, y] > largest)
                {
                    largest = arr[x, y];
                    count = 1;
                }
                else if (hits[x, y] == largest) ++count;
            }
        }
        // choose one at random
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (arr[x, y] == largest)
                {
                    if (rand.NextDouble() < (1.0 / count)) return new Point(x, y);
                    --count;
                }
            }
        }
        // shouldn't happen
        if (DEBUG) throw new Exception("GetLargest didn't choose anything!");
        else return new Point(0, 0);
    }

    // for debugging
    private void displayShips()
    {
        foreach (int w in remainingShips)
        {
            Console.Write("{0} ", w);
        }
        Console.WriteLine("");
    }

    private void displaySpaceState()
    {
        Console.WriteLine("Space");
        displayShips();
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (state[x, y] == State.UNKNOWN) Console.Write("{0,3:G} ", space[x, y]);
                else Console.Write("{0,3:G} ", stateChar[(int)state[x, y]]);
            }
            Console.Write("   ");
            for (int x = 0; x < width; ++x)
            {
                Console.Write("{0} ", stateChar[(int)state[x, y]]);
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");
        Console.WriteLine("");
    }

    private void displayHitState()
    {
        Console.WriteLine("Hits");
        displayShips();
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (state[x, y] == State.UNKNOWN) Console.Write("{0,3:G} ", hits[x, y]);
                else Console.Write("{0,3:G} ", stateChar[(int)state[x, y]]);
            }
            Console.Write("   ");
            for (int x = 0; x < width; ++x)
            {
                Console.Write("{0} ", stateChar[(int)state[x, y]]);
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");
        Console.WriteLine("");
    }
}
