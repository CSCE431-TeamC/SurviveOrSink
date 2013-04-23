namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Ship
    {
        private bool isPlaced = false;
        protected Point mLocation;
		private bool[] hitMarkers;
        protected ShipOrientation mOrientation;
        private int mLength;
        private ShipType mType;

        public Ship(ShipType type)
        {
			mType = type;
            /*if (length <= 1)
            {
                throw new ArgumentOutOfRangeException("length");
            }*/
			switch(mType) {
			case ShipType.AIRCRAFT_CARRIER:
				mLength = 5;
				break;
			case ShipType.BATTLESHIP:
				mLength = 4;
				break;
			case ShipType.DESTROYER:
				mLength = 3;
				break;
			case ShipType.SUBMARINE:
				mLength = 3;
				break;
			case ShipType.PATROL_BOAT:
				mLength = 2;
				break;
			}
			hitMarkers = new bool[mLength];
			for(int i = 0;i < mLength;i++) hitMarkers[i] = false;
        }
		public Ship(int length) {
			if (length <= 1)
            {
                throw new ArgumentOutOfRangeException("length");
            }
			mType = ShipType.CUSTOM;
			mLength = length;
			hitMarkers = new bool[mLength];
			for(int i = 0;i < mLength;i++) hitMarkers[i] = false;
		}

        public bool IsPlaced
        {
            get
            {
                return this.isPlaced;
            }
        }

        public Point Location
        {
            get
            {
                if (!this.isPlaced)
                {
                    throw new InvalidOperationException();
                }

                return this.mLocation;
            }
        }

        public ShipOrientation Orientation
        {
            get
            {
                if (!this.isPlaced)
                {
                    throw new InvalidOperationException();
                }

                return this.mOrientation;
            }
        }

        public int Length
        {
            get
            {
                return this.mLength;
            }
        }

        public bool Place(Point location, ShipOrientation orientation, Size boardSize)
        {
            this.mLocation = location;
            this.mOrientation = orientation;
            this.isPlaced = true;

            if (!IsValid(boardSize))
            {
                mLocation = new Point(-1, -1);
                isPlaced = false;
                return false;
            }
            return true;
        }
		
		public void Show() {
			if(mOrientation == ShipOrientation.Horizontal) {
				
			} else {
			}
		}

        public bool IsValid(Size boardSize)
        {
            if (!this.isPlaced)
            {
                return false;
            }

            if (this.mLocation.X < 0 || this.mLocation.Y < 0)
            {
                return false;
            }

            if (this.mOrientation == ShipOrientation.Horizontal)
            {
                if (this.mLocation.Y >= boardSize.Height || this.mLocation.X + this.mLength > boardSize.Width)
                {
                    return false;
                }
            }
            else
            {
                if (this.mLocation.X >= boardSize.Width || this.mLocation.Y + this.mLength > boardSize.Height)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAt(Point location)
        {
            if (this.mOrientation == ShipOrientation.Horizontal)
            {
                return (this.mLocation.Y == location.Y) && (this.mLocation.X <= location.X) && (this.mLocation.X + this.mLength > location.X);
            }
            else
            {
                return (this.mLocation.X == location.X) && (this.mLocation.Y <= location.Y) && (this.mLocation.Y + this.mLength > location.Y);
            }
        }
		
		public bool testHit(Point location) {
			int hitLocation = -1;
			
			if((mOrientation == ShipOrientation.Horizontal) && (location.Y == mLocation.Y)) {
				int diff = location.X - mLocation.X;
				if(diff >= 0 && diff < mLength) {
					hitLocation = diff;
				}
			} else if((mOrientation == ShipOrientation.Vertical) && (location.X == mLocation.X)) {
				int diff = location.Y - mLocation.Y;
				if(diff >= 0 && diff < mLength) {
					hitLocation = diff;
				}
			}
			
			if(hitLocation > -1) {
				hitMarkers[hitLocation] = true;
			}
			
			return (hitLocation > -1);
		}

        public IEnumerable<Point> GetAllLocations()
        {
            if (this.mOrientation == ShipOrientation.Horizontal)
            {
                for (int i = 0; i < this.mLength; i++)
                {
                    yield return new Point(this.mLocation.X + i, this.mLocation.Y);
                }
            }
            else
            {
                for (int i = 0; i < this.mLength; i++)
                {
                    yield return new Point(this.mLocation.X, this.mLocation.Y + i);
                }
            }
        }

        public bool ConflictsWith(Ship otherShip)
        {
            if (!otherShip.isPlaced) return false;
            foreach (var otherShipLocation in otherShip.GetAllLocations())
            {
                if (this.IsAt(otherShipLocation))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSunk()
        {
			foreach(bool hit in hitMarkers) {
				if(!hit) return false;
			}

            return true;
        }

        public virtual bool LoadModel()
        {
            return true;
        }
		
		public ShipType getShipType() {
		
			return mType;
			
		}
		
    }
}
