namespace Battleship
{
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var op1 = new SmartAI();
            var op2 = new RandomOpponent();

            BattleshipCompetition bc = new BattleshipCompetition(
                op1,
                op2,
                new Size(10, 10),       // Board Size
                2, 3, 3, 4, 5           // Ship Sizes
            );

            var winner = bc.RunCompetition();

            Console.WriteLine("{0} won the match!", winner.Name);

            Console.ReadKey(true);
        }
    }
}
