﻿using System;
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
            moveset[0] = new movement(new int[7]{0, 1, 0, 0, 0, 0, 0}, 0, 20, 0, "Slap");

            LinkedList<card> deck1 = new LinkedList<card>();
            LinkedList<card> deck2 = new LinkedList<card>();

            for (int i = 0; i < 7; i++)
            {
                deck1.AddFirst(new battler(0, 0, 40, 0, 0, 0, 0, 0, "Staryu", moveset));
                deck2.AddFirst(new battler(0, 0, 40, 0, 0, 0, 0, 0, "Staryu", moveset));
            }

            for (int i = 0; i < 7; i++)
            {
                deck1.AddFirst(new energy(0, 1, 1, "Basic Water Energy"));
                deck2.AddFirst(new energy(0, 1, 1, "Basic Water Energy"));
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