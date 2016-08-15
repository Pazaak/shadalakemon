using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;

namespace shandakemon.AI
{
    public class SimpleAI: DuelAI
    {
        bool modified;
        List<battler> basics;
        List<battler> evolutions;
        List<energy> energies;
        Player host;

        public SimpleAI(Player _host)
        {
            modified = true;
            host = _host;
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
                        if (btl.id == ev.evolvesFrom) scores[btl] += 1; // direct evolution
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

            basics.Remove((battler)selected);
            host.hand.Remove(selected);
            host.benched.Add((battler)selected); // Play the card
            Console.WriteLine(host.ToString() + " selects " + selected.ToString() + " as active pokemon");
            utils.Logger.Report(host.ToString() + " selects " + selected.ToString() + " as active pokemon");
        }

        public void SelectBenched()
        {
            if (modified) CheckModifications();

            Dictionary<battler, int> scores = new Dictionary<battler, int>();

            do
            {
                if (basics.Count() == 0) return; // No more basics
                scores.Clear();
                foreach (battler btl in basics)
                {
                    scores.Add(btl, 0);
                    foreach (battler ev in evolutions) // Checks for valid evolutions
                    {
                        if (btl.id == ev.evolvesFrom) scores[btl] += 1; // direct evolution
                        if (btl.id + 1 == ev.evolvesFrom) scores[btl] += 1; // second evolution (flawed if no evols followed by a family)
                    }

                    var filter = host.benched.Where(x => x.element == btl.element); // Check for battlers of the same element
                    if (filter.Count() == 0)
                        scores[btl] += 1;
                }

                battler selected = scores.FirstOrDefault(x => x.Value == scores.Values.Max() && x.Value > 0).Key;
                if (selected == null) return; // The best battler isn't good enough

                basics.Remove(selected);
                host.hand.Remove(selected);
                host.benched.Add(selected);
                Console.WriteLine(host.ToString() + " selects " + selected.ToString() + " as benched pokemon");
                utils.Logger.Report(host.ToString() + " selects " + selected.ToString() + " as benched pokemon");
            }
            while (scores.Values.Max() > 0);
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
