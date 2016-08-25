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
        List<trainer> trainers;
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
                    scores.Add(btl, EvaluateEvolutions(btl) + EvaluateEnergy(btl));

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
                scores = EvaluateBasic(basics);

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
            Dictionary<battler, int> scores = EvaluateActive(host.benched);

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
            Dictionary<battler, int> scores = EvaluateActive(target.benched);

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

        public bool MainPhase(Player opp)
        {
            Console.WriteLine(host.ToString() + " advances to main phase.");
            utils.Logger.Report(host.ToString() + " advances to main phase.");
            if (host.winCondition || opp.winCondition) return false;
            this.EvolutionPhase();
            if (host.winCondition || opp.winCondition) return false;
            this.SelectBenched();
            if (host.winCondition || opp.winCondition) return false;
            this.EnergyPhase();
            if (host.winCondition || opp.winCondition) return false;
            return this.CanAttack(opp);
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
                    scores.Add(btl, EvaluateAttachedEnergy(btl));

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
            trainers = new List<trainer>();
            battler temp1;
            energy temp2;
            trainer temp3;
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
                else // trainer
                {
                    temp3 = (trainer)c0;
                    trainers.Add(temp3);
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

        private Dictionary<battler, int> EvaluateActive(List<battler> argument)
        {
            // TODO: Take into account the opposing battlers
            Dictionary<battler, int> output = new Dictionary<battler, int>();

            foreach ( battler btl in argument )
                if ( btl != null )
                {
                    output.Add(btl, 0);
                    output[btl] += EvaluateAttachedEnergy(btl) * 30; // Heuristic multiplier
                    output[btl] += (btl.HP - btl.damage);
                }

            return output;
        }

        private Dictionary<battler, int> EvaluateBasic(List<battler> argument)
        {
            Dictionary<battler, int> output = new Dictionary<battler, int>();
            foreach (battler btl in argument)
                output.Add(btl, EvaluateEvolutions(btl) + EvaluateTypes(btl));
            return output;
        }

        private int EvaluateEvolutions(battler btl)
        {
            int score = 0;
            foreach (battler ev in evolutions) // Checks for valid evolutions
            {
                if (btl.id == ev.evolvesFrom) score += 1; // direct evolution
                if (btl.id + 1 == ev.evolvesFrom) score += 1; // second evolution (flawed if no evols followed by a family)
            }
            return score;
        }

        private int EvaluateEnergy(battler btl)
        {
            int score = 0;
            battler clone = btl.DeepCopy(); // Create a clone of the battler and add all the energies
            foreach (energy en in energies)
                clone.attachEnergy(en);
            foreach (movement mov in clone.movements)
                if (clone.isUsable(mov)) score+= 1; // Check for usable movements
            return score;
        }

        private int EvaluateTypes(battler btl)
        {
            var filter = host.benched.Where(x => x.element == btl.element); // Check for battlers of the same element
            return filter.Count() == 0 ? 1 : 0;
        }

        private int EvaluateAttachedEnergy(battler btl)
        {
            int energySum = btl.energies.Count;
            int lastIndex = btl.movements.Length - 1;
            int movCost = btl.movements[lastIndex].cost.Sum();

            if (energySum >= movCost)
                return energySum - movCost; // Enough or excess of energy

            int result = 0;

            while ( lastIndex >= 0 && energySum < btl.movements[lastIndex].cost.Sum() )
            {
                result--;
                lastIndex--;
            }

            return result;

        }

        private void EvolutionPhase()
        {
            // TODO: Do this only when necessary
            this.CheckModifications();

            bool breeder = trainers.Any(x => x.effect == 6); // Has pkm breeder in hand

            List<battler> candidates;
            foreach ( battler evo in evolutions )
            {
                candidates = new List<battler>();
                candidates = candidates.Concat(host.benched.Where(x => x.id == evo.evolvesFrom).ToList()).ToList();
                if (breeder)
                    candidates = candidates.Concat(host.benched.Where(x => x.id+1 == evo.evolvesFrom).ToList()).ToList();

                if ( candidates.Count > 0)
                {
                    Dictionary<battler, int> scores = EvaluateActive(candidates);

                    if ( scores.Values.Max() > 0 )
                    {
                        battler target = scores.FirstOrDefault(x => x.Value == scores.Values.Max()).Key;

                        if (host.benched[0] == target)
                            host.benched[0] = evo;
                        else
                        {
                            host.benched.Remove(target);
                            host.benched.Add(evo);
                        }

                        if (target.id + 1 == evo.evolvesFrom)
                        {
                            card bree = trainers.FirstOrDefault(x => x.effect == 6);
                            Console.WriteLine(host.ToString() + " used " + bree.ToString() + ".");
                            utils.Logger.Report(host.ToString() + " used " + bree.ToString() + ".");
                            host.CardToDiscard(bree);
                        }

                        evo.evolve(target);
                    }
                }
            }   
        }

        private void EnergyPhase()
        {
            // TODO
            CheckModifications();

            if (energies.Count == 0) // No energies in hand
                return;

            bool[] types = new bool[Constants.NTypes];

            Dictionary<battler, int> negScores = new Dictionary<battler, int>();
            Dictionary<battler, int> posScores = new Dictionary<battler, int>();
            int score;
            foreach (battler btl in host.benched)
            {
                score = EvaluateAttachedEnergy(btl);
                if (score < 0 && !negScores.ContainsKey(btl)) negScores.Add(btl, score);
                else if ( !posScores.ContainsKey(btl)) posScores.Add(btl, score);
            }
            
            if ( negScores.Count == 0 ) // All battlers have excess of energy
            {
                Dictionary<battler, int> typeMatch = posScores.Where(x => types[x.Key.element]).ToDictionary(x => x.Key, x => x.Value);

                if ( typeMatch.Count > 0) // There is type match among
                {
                    battler selected = typeMatch.FirstOrDefault(x => x.Value <= typeMatch.Values.Min()).Key;
                    energy en = energies.FirstOrDefault(x => x.elem == selected.element);
                    selected.attachEnergy(en);
                    host.hand.Remove(en);
                    Console.WriteLine(en.name + " attached to " + selected.ToString());
                    utils.Logger.Report(en.name + " attached to " + selected.ToString());
                }
                else // There are no type matches
                {
                    battler selected = posScores.FirstOrDefault(x => x.Value <= posScores.Values.Min()).Key;
                    energy en = energies.First();
                    selected.attachEnergy(en);
                    host.hand.Remove(en);
                    Console.WriteLine(en.name + " attached to " + selected.ToString());
                    utils.Logger.Report(en.name + " attached to " + selected.ToString());
                }
            }
            else // Battlers need more energy
            {
                Dictionary<battler, int> typeMatch = negScores.Where(x => types[x.Key.element]).ToDictionary(x => x.Key, x => x.Value);

                if (typeMatch.Count > 0) // There is type match among
                {
                    battler selected = typeMatch.FirstOrDefault(x => x.Value >= typeMatch.Values.Max()).Key;
                    energy en = energies.FirstOrDefault(x => x.elem == selected.element);
                    selected.attachEnergy(en);
                    host.hand.Remove(en);
                    Console.WriteLine(en.name + " attached to " + selected.ToString());
                    utils.Logger.Report(en.name + " attached to " + selected.ToString());
                }
                else // There are no type matches
                {
                    battler selected = negScores.FirstOrDefault(x => x.Value >= negScores.Values.Max()).Key;
                    energy en = energies.First();
                    selected.attachEnergy(en);
                    host.hand.Remove(en);
                    Console.WriteLine(en.name + " attached to " + selected.ToString());
                    utils.Logger.Report(en.name + " attached to " + selected.ToString());
                }
            }
        }

        private bool CanAttack(Player opp)
        {
            battler front = host.benched[0];
            front.BattleDescription();

            if (front.movements.Length == 0 || !front.isUsable(front.movements[0]))
            {

                return false;
            }
            else if (front.status == 1 || front.status == 2) // Statused
            {
                Console.WriteLine("Front pokemon is " + utilities.numToStatus(front.status) + " and can't attack");
                return false;
            }
            else if (front.status == 3) // Confusion check
            {
                Console.WriteLine("Confusion check:");
                utils.Logger.Report("Confusion check:");
                if (CRandom.RandomInt() < 0)
                {
                    Console.WriteLine(host.ToString() + " won the coin flip.");
                    utils.Logger.Report(host.ToString() + " won the coin flip.");
                    utils.Logger.Report(host.ToString() + " advances to attack phase.");
                    return true;
                }
                else
                {
                    Console.WriteLine(host.ToString() + " lost the coin flip.");
                    utils.Logger.Report(host.ToString() + " lost the coin flip.");
                    effects.damage(Constants.TNone, 30, front, null, host, opp);
                    utils.Logger.Report(host.ToString() + " ends the turn without attacking.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine(host.ToString() + " advances to attack phase.");
                utils.Logger.Report(host.ToString() + " advances to attack phase.");
                return true; // Advance to attack phase
            }
        }
    }
}
