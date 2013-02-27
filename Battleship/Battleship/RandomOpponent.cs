namespace Battleship
{
    using System;
    using System.Collections.ObjectModel;

    public class RandomOpponent : IBattleshipOpponent
    {
        public string Name { get { return "Random"; } }

        Random rand = new Random();
        Size gameSize;

        public void NewGame(Size size)
        {
            this.gameSize = size;
        }

        public void PlaceShips(ReadOnlyCollection<Ship> ships)
        {
            foreach (Ship s in ships)
            {
                s.Place(
                    new Point(
                        rand.Next(this.gameSize.Width),
                        rand.Next(this.gameSize.Height)),
                    (ShipOrientation)rand.Next(2));
            }
        }

        public Point GetShot()
        {
            return new Point(
                rand.Next(this.gameSize.Width),
                rand.Next(this.gameSize.Height));
        }

        public void NewMatch(string opponent) { }
        public void OpponentShot(Point shot) { }
        public void ShotHit(Point shot, bool sunk) { }
        public void ShotMiss(Point shot) { }
        public void GameWon() { }
        public void GameLost() { }
        public void MatchOver() { }
    }
}
