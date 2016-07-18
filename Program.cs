using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;

namespace shandakemon
{
    class Program
    {
        static void Main(string[] args)
        {
            movement[] moveset = new movement[1];
            moveset[0] = new movement(new int[7]{0, 1, 0, 0, 0, 0, 0}, 1, 0, "Slap", 20, 0);

            movement[] moveset2 = new movement[2];
            moveset2[0] = new movement(new int[7] { 0, 1, 0, 0, 0, 0, 0 }, 1, 2, "Bubble", 10, 1);
            moveset2[1] = new movement(new int[7] { 1, 1, 0, 0, 0, 0, 0 }, 1, 3, "Withdraw", Legacies.fog, 1);

            LinkedList<card> deck1 = new LinkedList<card>();
            LinkedList<card> deck2 = new LinkedList<card>();

            for (int i = 0; i < 10; i++)
            {
                deck1.AddFirst(new battler(0, 1, 40, 6, 2, 0, 0, 1, "Staryu", 120, 0, moveset));
                deck2.AddFirst(new battler(0, 1, 40, 6, 2, 0, 0, 1, "Squirtle", 7, 0, moveset2));
            }

            for (int i = 0; i < 20; i++)
            {
                deck1.AddFirst(new energy(0, 1, 1, "Basic Water Energy"));
                deck2.AddFirst(new energy(0, 1, 1, "Basic Water Energy"));
            }

            movement[] moveset1 = new movement[2];
            moveset1[0] = new movement(new int[7] { 0, 2, 0, 0, 0, 0, 0 }, 1, 1, "Recover", 1, 0);
            moveset1[1] = new movement(new int[7] { 2, 1, 0, 0, 0, 0, 0 }, 1, 2, "Star Freeze", 20, 1);

            movement[] moveset3 = new movement[2];
            moveset3[0] = new movement(new int[7] { 1, 1, 0, 0, 0, 0, 0 }, 1, 3, "Withdraw", Legacies.fog, 1);
            moveset3[1] = new movement(new int[7] { 2, 1, 0, 0, 0, 0, 0 }, 1, 0, "Bite", 40, 0);

            for (int i = 0; i < 10; i++)
            {
                deck1.AddFirst(new battler(1, 1, 60, 6, 2, 0, 0, 1, "Starmie", 121, 120, moveset1));
                deck2.AddFirst(new battler(1, 1, 70, 6, 2, 0, 0, 1, "Wartortle", 8, 7, moveset3));
            }

            Player player1 = new Player(1, deck1, 2);
            Player player2 = new Player(2, deck2, 2);

            duel dd = new duel(player1, player2);
            dd.battleFlow();

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
