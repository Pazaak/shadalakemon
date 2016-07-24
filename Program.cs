using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;
using Newtonsoft.Json;

namespace shandakemon
{
    class Program
    {
        static void Main(string[] args)
        {
            DBBuilder.energies();
            DBBuilder.pokemon();

            string raw_energy = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\energies");
            string raw_pokemon = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\pokemon");

            energy[] energies = JsonConvert.DeserializeObject<energy[]>(raw_energy);
            battler[] battlers = JsonConvert.DeserializeObject<battler[]>(raw_pokemon);

            LinkedList <card> deck1 = new LinkedList<card>(); // Creates a deck
            LinkedList<card> deck2 = new LinkedList<card>();

            for (int i = 0; i < 20; i++) // Energies
            {
                deck1.AddFirst(energies[7].DeepCopy());
                deck2.AddFirst(energies[7].DeepCopy());

                if ( i < 6 ) // Staryu
                {
                    deck1.AddFirst(battlers[64].DeepCopy());
                }

                if ( i < 4) // Squirtle and Starmie
                {
                    deck1.AddFirst(battlers[62].DeepCopy());
                    deck1.AddFirst(battlers[63].DeepCopy());
                }

                if ( i < 3 ) // Wartortle and Blastoise
                {
                    deck1.AddFirst(battlers[1].DeepCopy());
                    deck1.AddFirst(battlers[41].DeepCopy());
                }

                if ( i < 20 )
                {
                    deck2.AddFirst(battlers[56].DeepCopy());
                }
            }

            Player player1 = new Player(1, deck1, 2);
            Player player2 = new Player(2, deck2, 2); // Creates the players

            duel dd = new duel(player1, player2); // Creates the duel
            dd.battleFlow(); // Starts the battle

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
