namespace Battleship
{
    using System;
    using System.Collections.ObjectModel;

    public interface IBattleshipOpponent
    {
        string Name
        {
            get;
        }

        void NewMatch(string opponent);

        void NewGame(Size size);

        void PlaceShips(ReadOnlyCollection<Ship> ships);

        Point GetShot();

        void OpponentShot(Point shot);

        void ShotHit(Point shot, bool sunk);

        void ShotMiss(Point shot);

        void GameWon();

        void GameLost();

        void MatchOver();
    }
}
