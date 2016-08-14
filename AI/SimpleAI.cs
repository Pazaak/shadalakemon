using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;

namespace shandakemon.AI
{
    class SimpleAI
    {
        Player host;
        bool modified;
        List<battler> basics;
        List<battler> evolutions;
        List<energy> energies;


        public SimpleAI(Player dhost)
        {
            host = dhost;
            modified = true;
        }
        
        public void SelectActive()
        {
            if (modified) CheckModifications();

            /*
            p1.hand.Remove(selected);
            p1.benched.Add((battler)selected); // Play the card
            Console.WriteLine(selected.ToString() + " selected as active pokemon");
            utils.Logger.Report(p1.ToString() + " selects " + selected.ToString() + " as active pokemon");*/
        } 

        private void CheckModifications()
        {
            basics = new List<battler>();
            evolutions = new List<battler>();
            energies = new List<energy>();
            battler temp1;
            energy temp2;
            foreach (card c0 in host.hand)
            {
                if (c0.getSuperType() == 0) // Check if it is a battler
                {
                    temp1 = (battler)c0;
                    if (temp1.type == 0) // Basic
                        basics.Add(temp1);
                    else
                        evolutions.Add(temp1);
                }
                else if (c0.getSuperType() == 2) // Energy type
                {
                    temp2 = (energy)c0;
                    energies.Add(temp2);
                }
            }

            modified = false;
        }
    }
}
