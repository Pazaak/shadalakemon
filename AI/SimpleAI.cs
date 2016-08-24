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

        public void SelectMovement(Player opp)
        {
            movement[] movelist = host.benched[0].movements;
            int[] scoreTable = new int[movelist.Length];

            int lastMax = int.MinValue;
            int lastIndex = int.MinValue;
            int newMax = 0;
            host.benched[0].BattleDescription();
            for (int i = 0; i < movelist.Length; i++)
            {
                if (movelist[i].usable)
                    newMax = i + EvaluateMovement(movelist[i], opp);
                else
                    newMax = -1;   

                if (newMax > lastMax)
                {
                    lastMax = newMax;
                    lastIndex = i;
                }
            }
            
            host.benched[0].execute(lastIndex, host, opp, opp.benched[0]);
        }

        public void PriceProcedure()
        {
            bool[] prices = host.listPrices(); // Checks available prices
            int i = 0;
            while (!prices[i]) i++; 

            host.drawPrice(i);  // Draws the price
            prices[i] = false;

            if (prices.All(x => x == false))
            {
                Console.WriteLine("No more price cards. " + host.id + " wins."); // Makes the player win
                utils.Logger.Report(host.ToString() + " has no more price cards. " + host.ToString() + " wins.");
                host.winCondition = true;
            }
        }

        public void KnockoutProcedure()
        {
            Dictionary<battler, int> scores = EvaluateBenched(host);

            battler selected = scores.FirstOrDefault(x => x.Value >= scores.Values.Max<int>()).Key;

            host.toFront(selected);
        }

        public void DiscardEnergy(Player caller, Player t_cont, battler target, int type, int quantity)
        {
            energy temp;
            if ( type != -1 || (type == Constants.TFire && target.conditions.ContainsKey(Legacies.energyBurn)) )
            {
                for (int i = 0; i < quantity; i++)
                {
                    temp = target.energies.FirstOrDefault(x => x.elem == type);
                    t_cont.discardEnergy(target, temp);
                }
            }
            else
            {
                int counter = 0;
                while ( counter++ < quantity)
                {
                    temp = target.energies.First(x => x.elem == type);
                    t_cont.discardEnergy(target, temp);
                }
            }
        }

        public void DeactivateMovement(Player target_controller, battler target)
        {
            // TODO: Do this with some intelligence
            int index;
            if ( target_controller == host)
            {
                index = target.movements.Length - 1;
                while (!target.movements[index].usable && index >= 0) index--;
            }
            else
            {
                index = 0;
                while (!target.movements[index].usable && index < target.movements.Length) index++;
            }

            effects.addCondition(target, Legacies.deacMov, new int[2] { 2, index });
        }

        public void Wheel(Player target)
        {
            Dictionary<battler, int> scores = EvaluateBenched(target);

            battler selected = null;
            if (target == host)
                selected = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;
            else
                selected = scores.FirstOrDefault(x => x.Value >= scores.Values.Min()).Key;

            target.ExchangePosition(target.benched.LastIndexOf(selected));
        }

        public void ChangeResistance(Player opp, battler target)
        {
            // TODO: Improve AI
            if ( opp == host ) // Changing the resistance of the opponent
                target.res_elem = host.benched[0].element + 1 % 7;
            else // Changing own resistance
                target.res_elem = opp.benched[0].element;
        }

        public void ChangeWeakness(Player opp, battler target)
        {
            // TODO: Improve AI
            if (opp == host) // Changing the weakness of the opponent
                target.res_elem = host.benched[0].element;
            else // Changing own weakness
                target.res_elem = opp.benched[0].element + 1 % 7;
        }

        public void CastMovement(Player opp)
        {
            battler target = opp.benched[0];
            int index = target.movements.Length - 1;
            while (!target.movements[index].usable && index >= 0) index--;

            target.movements[index].execute(host, opp, host.benched[0], target, true);
        }

        public battler ChooseBattler(bool maxHP = true, bool maxEnergy = true)
        {
            Dictionary<battler, int> scores = new Dictionary<battler, int>();

            foreach (battler btl in host.benched)
            {
                scores.Add(btl, (btl.HP - btl.damage) / 10);
                scores[btl] = maxHP ? scores[btl] : -scores[btl];
                scores[btl] += maxEnergy ? btl.energies.Count*2 : -btl.energies.Count*2;
            }

            return scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;
        }

        public battler ChooseForDiscard(Player source, int quantity, bool max)
        {
            // TODO: Only take in to account those who meet the energy requirements
            Dictionary<battler, int> scores = new Dictionary<battler, int>();
            foreach (battler btl in source.benched)
                if ( btl.energies.Count < quantity)
                    scores.Add(btl, max? int.MinValue : int.MaxValue);
                else
                    scores.Add(btl, EvaluateEnergy(btl));

            if (max) // Best score
                return scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;
            else
                return scores.FirstOrDefault(x => x.Value <= scores.Values.Min()).Key;
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

        private int EvaluateMovement(movement cast, Player opp)
        {
            // TODO: Do movement evaluations
            return 0;
        }

        // Scores the current damage
        private int DamageTiers(int max, int curr)
        {
            if (max < curr << 2)
                return -5;
            int result = 1;
            result += (int)(max * 0.75) < curr ? 2 : 0;
            result += (int)(max * 0.9) < curr ? 2 : 0;
            return result;
        }

        // Scores the current energy holding
        private int EnergyTiers(battler source, movement cast)
        {
            int totalCost = cast.cost.Sum();
            int totalProduction = source.energyTotal.Sum();

            if (totalCost <= totalProduction) return 0;
            int output = 0;
            output += (int)totalCost * 1.25 >= totalProduction ? 1 : 0;
            output += (int)totalCost * 1.5 >= totalProduction ? 1 : 0;

            return output;
        }

        private Dictionary<battler, int> EvaluateBenched(Player target)
        {
            // TODO: Take into account the opposing battlers
            Dictionary<battler, int> output = new Dictionary<battler, int>();

            foreach ( battler btl in target.benched )
                if ( btl != null )
                {
                    output.Add(btl, 0);
                    output[btl] += EvaluateEnergy(btl) * 30; // Heuristic multiplier
                    output[btl] += (btl.HP - btl.damage);
                }

            return output;
        }

        private int EvaluateEnergy(battler btl)
        {
            int energySum = btl.energies.Count;
            int lastIndex = btl.movements.Length - 1;
            int movCost = btl.movements[lastIndex].cost.Sum();

            if (energySum >= movCost)
                return energySum - movCost; // Enough or excess of energy

            int result = 0;

            while ( energySum < btl.movements[lastIndex].cost.Sum() && lastIndex >= 0 )
            {
                result--;
                lastIndex--;
            }

            return result;

        }
    }
}
