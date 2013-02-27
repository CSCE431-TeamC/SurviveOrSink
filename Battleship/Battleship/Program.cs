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
                1,                     // Wins per match
                true,                   // Play out?
                new Size(10, 10),       // Board Size
                2, 3, 3, 4, 5           // Ship Sizes
            );

            var scores = bc.RunCompetition();

            foreach (var key in scores.Keys.OrderByDescending(k => scores[k]))
            {
                Console.WriteLine("{0}: {1}", key.Name, scores[key]);
            }

            Console.ReadKey(true);
        }
    }
}
