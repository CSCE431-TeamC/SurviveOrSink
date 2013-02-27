namespace Battleship
{
    using System;
    using System.Collections.ObjectModel;

    public class HumanPlayer : IBattleshipOpponent
    {
        public string Name { get { return "Human"; } }

        //initialize things here
        public void NewGame(Size size)
        {
        }

        //prompt for user input for ship locations
        public void PlaceShips(ReadOnlyCollection<Ship> ships)
        {
        }

        //prompt user input for shot location
        public Point GetShot()
        {
            return new Point(0,0);
        }

        public void OpponentShot(Point shot) { }
        public void ShotHit(Point shot, bool sunk) { }
        public void ShotMiss(Point shot) { }
        public void GameWon() { }
        public void GameLost() { }
    }
}
