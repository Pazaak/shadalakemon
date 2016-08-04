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
            DBBuilder.trainers();

            string raw_energy = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\energies");
            string raw_pokemon = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\pokemon");
            string raw_trainers = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\trainers");

            energy[] energies = JsonConvert.DeserializeObject<energy[]>(raw_energy);
            battler[] battlers = JsonConvert.DeserializeObject<battler[]>(raw_pokemon);
            trainer[] trainers = JsonConvert.DeserializeObject<trainer[]>(raw_trainers);

            List<card> deck1 = new List<card>(); // Creates a deck
            List<card> deck2 = new List<card>();

            for (int i = 0; i < 20; i++) // Energies
            {
                deck1.Add(energies[6].DeepCopy());

                if ( i < 20)
                    deck2.Add(energies[4].DeepCopy());
                if ( i < 0 )
                    deck2.Add(energies[6].DeepCopy());

                if ( i < 6 )
                {
                    deck1.Add(battlers[58].DeepCopy());
                    deck1.Add(trainers[2].DeepCopy());
                }

                if ( i < 4 )
                {
                    deck1.Add(battlers[37].DeepCopy());
                    deck1.Add(battlers[12].DeepCopy());
                }

                if ( i < 0 )
                {
                    deck1.Add(battlers[12].DeepCopy());
                    deck1.Add(battlers[37].DeepCopy());
                }

                if ( i < 8 )
                {
                    deck2.Add(battlers[52].DeepCopy());
                }

                if ( i < 12 )
                {
                    deck2.Add(trainers[1].DeepCopy());
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
