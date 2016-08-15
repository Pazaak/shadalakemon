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

            card selected;
            if (basics.Count == 1) // Only one option
                selected = basics[0];
            else // Many options
            {
                Dictionary<battler, int> scores = new Dictionary<battler, int>();
                foreach (battler btl in basics)
                {
                    scores.Add(btl, 0);
                    foreach (battler ev in evolutions) // Checks for valid evolutions
                    {
                        if (btl.id == ev.evolvesFrom) scores[btl] += 2; // direct evolution
                        if (btl.id + 1 == ev.evolvesFrom ) scores[btl] += 1; // second evolution (flawed if no evols followed by a family)
                    }
                    battler clone = btl.DeepCopy(); // Create a clone of the battler and add all the energies
                    foreach (energy en in energies)
                        clone.attachEnergy(en);
                    foreach (movement mov in clone.movements)
                        if (clone.isUsable(mov)) scores[btl] += 1; // Check for usable movements
                }

                selected = scores.FirstOrDefault(x => x.Value == scores.Values.Max()).Key;
            }

            host.hand.Remove(selected);
            host.benched.Add((battler)selected); // Play the card
            Console.WriteLine(selected.ToString() + " selected as active pokemon");
            utils.Logger.Report(host.ToString() + " selects " + selected.ToString() + " as active pokemon");
            modified = true;
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
