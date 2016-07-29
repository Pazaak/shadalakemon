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
                deck2.AddFirst(energies[3].DeepCopy());

                if ( i < 6 )
                {
                    deck1.AddFirst(battlers[38].DeepCopy());
                }

                if ( i < 4)
                {
                    deck1.AddFirst(battlers[58].DeepCopy());
                    deck1.AddFirst(battlers[38].DeepCopy());
                }

                if ( i < 3 )
                {
                    deck1.AddFirst(battlers[12].DeepCopy());
                    deck1.AddFirst(battlers[37].DeepCopy());
                }

                if ( i < 20 )
                {
                    deck2.AddFirst(battlers[67].DeepCopy());
                }

                if ( i < 0 )
                {
                    deck2.AddFirst(battlers[17].DeepCopy());
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
