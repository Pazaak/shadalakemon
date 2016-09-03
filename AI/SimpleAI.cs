using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shandakemon.core;

namespace shandakemon.AI
{
    public class SimpleAI: DuelAI
    {
        bool modified, peek;
        List<battler> basics;
        List<battler> evolutions;
        List<energy> energies;
        List<trainer> trainers;
        Player host;

        public SimpleAI(Player _host, bool _peek = false)
        {
            modified = true;
            peek = _peek;
            host = _host;
        }
        
        public void SelectActive()
        {
            if (modified) CheckModifications();
            if (peek) Console.WriteLine(host.ShowHand());

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
            int threshold;
            do
            {
                if (basics.Count() == 0) return; // No more basics
                scores = EvaluateBasic(basics);

                threshold = host.benched.Count == 1 ? -2 : 0;
                battler selected = scores.FirstOrDefault(x => x.Value == scores.Values.Max() && x.Value > threshold).Key;
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

            target.ExchangePosition(selected);
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
            if (peek) Console.WriteLine(host.ShowHand());
            if (host.winCondition || opp.winCondition) return false;
            this.EvolutionPhase();
            if (host.winCondition || opp.winCondition) return false;
            this.SelectBenched();
            if (host.winCondition || opp.winCondition) return false;
            this.EnergyPhase();
            if (host.winCondition || opp.winCondition) return false;
            this.PowerPhase(opp);
            if (host.winCondition || opp.winCondition) return false;
            this.RetreatPhase(opp);
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

        private Dictionary<card, int> EvaluateCards(List<card> target)
        {
            Dictionary<card, int> output = new Dictionary<card, int>();

            // TODO: Do this seriously
            foreach (card ca in target)
                output.Add(ca, CRandom.RandomInt());

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

        private energy BestEnergy(battler btl)
        {
            List<energy> attached = btl.energies;

            // Try to find the energies of the same type as the battler
            energy selected = attached.FirstOrDefault(x => x.elem == btl.element);

            if (selected == null) // No match was found
                selected = attached.FirstOrDefault(x => x.quan > 1); // Return that produces more than one

            if (selected == null) // No one produces more than one
                selected = attached.First(); // Return any

            return selected; 
        }

        private energy WorstEnergy(battler btl)
        {
            List<energy> attached = btl.energies;

            // Find a not matching element energy with a production of one
            energy selected = attached.FirstOrDefault(x => x.elem != btl.element && x.quan <= 1);

            if (selected == null)
                selected = attached.FirstOrDefault(x => x.elem != btl.element); // Search for one which produces more than one

            if (selected == null) // No match was found
                selected = attached.First(); // Return anyone

            return selected;
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

        private void PowerPhase(Player opp)
        {
            List<battler> powers = host.benched.Where(x => x.power != null).ToList();
            bool end = false; // No power was used in the last iteration

            while ( !end )
            {
                end = true;
                foreach (battler btl in powers)
                {
                    switch ( btl.power.effect )
                    {
                        case 0: // Rain dance
                            end = end && !this.RainDance(btl);
                            break;
                        case 1: // Energy trans
                            end = end && !this.EnergyTrans(btl);
                            break;
                        case 2: // Damage swap
                            end = end && !this.DamageSwap(btl);
                            break;
                        case 3: // Buzzap
                            end = end && !this.Buzzap(btl, opp);
                            break;
                    }
                }
            }

        }

        private void RetreatPhase(Player opp)
        {
            if (host.benched.Count < 2) return; // Not usable
            battler front = host.benched[0];
            if (!front.canRetreat()) return; // Not usable

            Dictionary<battler, int> scores = EvaluateActive(host.benched);

            int frontScore = scores[front]; // Remove front
            scores.Remove(front);

            if (!scores.Any(x => x.Value > frontScore + front.retreat*10)) return; // Not worth it

            battler selected = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;

            host.ExchangePosition(selected);
        }

        private void TrainerPhase(Player opp)
        {
            
            bool end;

            do
            {
                // TODO
                CheckModifications();
                end = true;

                foreach( trainer tra in trainers)
                {
                    switch(tra.effect)
                    {
                        case 0:
                            end &= !ClefairyDoll(tra);
                            break;
                        case 1:
                            end &= !ComputerSearch(tra);
                            break;
                        case 2:
                            end &= !DevolutionSpray();
                            break;
                        case 3:
                            end &= !ImposterOak(tra, opp);
                            break;
                        case 4:
                            end &= !ItemFinder(tra);
                            break;
                        case 5:
                            end &= !Lass(opp);
                            break;
                        // Case 6: Pokemon Breeder, implemented as a part of the evolution procedures
                        case 7:
                            end &= !PokemonTrader(tra);
                            break;
                        case 8:
                            end &= !ScoopUp(tra, opp);
                            break;
                        case 9:
                            end &= !SuperEnergyRemoval(tra, opp);
                            break;
                        case 10:
                            end &= !Defender(tra);
                            break;
                        case 11:
                            end &= !EnergyRetrieval(tra);
                            break;
                        case 12:
                            end &= !FullHeal(tra);
                            break;
                        case 13:
                            end &= !Maintenance(tra);
                            break;
                        case 14:
                            end &= !PlusPower(tra);
                            break;
                        case 15:
                            end &= !PokemonCenter(tra);
                            break;
                        case 16:
                            end &= !PokemonFlute(tra, opp);
                            break;
                        case 17:
                            end &= !Pokedex(tra);
                            break;
                        case 18:
                            end &= !ProfesorOak(tra);
                            break;
                        case 19:
                            end &= !Revive(tra);
                            break;
                        case 20:
                            end &= !SuperPotion(tra);
                            break;
                        case 21:
                            end &= !Bill(tra);
                            break;
                        case 22:
                            end &= !EnergyRemoval(tra, opp);
                            break;
                        case 23:
                            end &= !GustOfWind(tra, opp);
                            break;
                        case 24:
                            end &= !Potion(tra);
                            break;
                        case 25:
                            end &= !Switch(tra);
                            break;
                    }
                }

            } while (!end);
        }

        #region Powers
        private bool RainDance(battler caller)
        {
            CheckModifications();
            // Check for needs of energy
            Dictionary<battler, int> scores = new Dictionary<battler, int>();

            int score;
            foreach (battler btl in host.benched)
            {
                score = EvaluateAttachedEnergy(btl);
                if (btl.element == Constants.TWater && score < 0 && !scores.ContainsKey(btl))
                    scores.Add(btl, score);
            }

            if (scores.Count == 0) return false; // No need of energy

            if (energies.Any(x => x.elem == Constants.TWater)) return false; // No energy of the selected type

            // Activating Rain Dance
            battler selected = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;
            energy en = energies.FirstOrDefault(x => x.elem == Constants.TWater);
            selected.attachEnergy(en);
            host.hand.Remove(en);
            utils.Logger.Report(caller.ToString() + " uses " + caller.power.name + ".");
            Console.WriteLine(caller.ToString() + " uses " + caller.power.name + ".");
            utils.Logger.Report(en.name + " attached to " + selected.ToString());
            Console.WriteLine(en.name + " attached to " + selected.ToString());
            return true;
        }

        private bool EnergyTrans(battler caller)
        {
            if (host.benched.Count < 2) return false; // Cannot transfer energy with only 1

            // Calculate energy scores
            Dictionary<battler, int> negScores = new Dictionary<battler, int>();
            Dictionary<battler, int> posScores = new Dictionary<battler, int>();
            int score;
            foreach (battler btl in host.benched)
            {
                score = EvaluateAttachedEnergy(btl);
                if (score < 0 && !negScores.ContainsKey(btl)) negScores.Add(btl, score);
                else if ( score > 0 && !posScores.ContainsKey(btl)) posScores.Add(btl, score);
            }

            battler source = null; // Check for benched dying battler
            energy en = null;
            for (int i = 1; i < host.benched.Count; i++)
                if ( host.benched[i].HP * 0.9 <= host.benched[i].damage && host.benched[i].energies.Any() )
                {
                    source = host.benched[i];
                    break;
                }

            if ( source != null)
                // Energy transfering commence
                en = source.energies.First(); // Any energy will do
            else // Not dying battler, check for excess of energy
            {
                if (posScores.Any())
                    return false; // No excess of energy on any battler

                // Select the battler with most excess of energy
                source = posScores.FirstOrDefault(x => x.Value >= posScores.Values.Max()).Key;

                // Try to select an energy which is not from the battler's type
                en = source.energies.FirstOrDefault(x => x.elem != source.element);
                if (en == null) // Else, get one of the same type
                    en = source.energies.First();
            }

            // Look for battler of the same type as the energy
            Dictionary<battler, int> typedNeg = negScores.Where(x => x.Key.element == en.elem).ToDictionary(x => x.Key, x => x.Value);
            battler target = null;
            if (typedNeg.Any()) // At least one negative score has the energy's type
                target = typedNeg.FirstOrDefault(x => x.Value >= typedNeg.Values.Max()).Key;
            else // None of the negative scored battlers has the same type as the energy
                target = negScores.FirstOrDefault(x => x.Value >= negScores.Values.Max()).Key;

            target.attachEnergy(en);
            source.energies.Remove(en);
            utils.Logger.Report(caller.ToString() + " uses " + caller.power.name + ".");
            Console.WriteLine(caller.ToString() + " uses " + caller.power.name + ".");
            utils.Logger.Report(en.name + " de-attached from " + source.ToString());
            Console.WriteLine(en.name + " de-attached from " + source.ToString());
            utils.Logger.Report(en.name + " attached to " + target.ToString());
            Console.WriteLine(en.name + " attached to " + target.ToString());
            return true;

        }

        private bool DamageSwap(battler caller)
        {
            if (host.benched.Count < 2) return false; // Not enough battlers
            if (host.benched.All(x => x.damage == 0)) return false; // All undamaged

            battler source;
            battler target;
            if (host.benched[0].damage > 0) // Front damaged
            {
                source = host.benched[0]; // Source of damage is front
                int mindam = int.MaxValue;
                foreach (battler btl in host.benched) // Get min
                    if (btl.damage < mindam)
                        mindam = btl.damage;
                target = host.benched.FirstOrDefault(x => x.damage <= mindam); // Find min
                if (target.damage + 10 == target.HP) return false; // Min on the edge of its life
            }
            else
            {
                source = host.benched.FirstOrDefault(x => x.damage == x.HP - 10);
                if (source == null) return false; // No one is in the edge of death

                target = host.benched.FirstOrDefault(x => x.damage < x.HP - 10);
                if (target == null) return false; // Every one else is in the edge of death
            }

            source.damage -= 10;
            target.damage += 10;
            utils.Logger.Report(caller.ToString() + " uses " + caller.power.name + ".");
            Console.WriteLine(caller.ToString() + " uses " + caller.power.name + ".");
            Console.WriteLine(source.ToString() + " losses a damage counter. " + target.ToString() + " obtains a damage counter.");
            utils.Logger.Report(source.ToString() + " losses a damage counter. " + target.ToString() + " obtains a damage counter.");
            return true;
        }

        private bool Buzzap(battler source, Player opp)
        {
            if (host.benched.Count < 2) return false; // Not enough battlers to work

            int score = (source.HP - source.damage) / 10 - 1;
            score += source.energies.Count >= 3 ? 3 : source.energies.Count;

            if (score > 3) return false; // Not enough crippled to activate

            host.ToDiscard(source);
            host.discarded.Remove(source);
            opp.PriceProcedure();

            battler target = null;
            int minsco = int.MaxValue;
            foreach (battler btl in host.benched)
                if ( btl != null && EvaluateAttachedEnergy(btl) < minsco)
                    target = btl;

            energy attEnergy = new energy(1, target.element, source.power.parameters[0], source.name, source);

            target.attachEnergy(attEnergy);
            
            host.KnockoutProcedure(opp);
            utils.Logger.Report(source.ToString() + " uses " + source.power.name + ".");
            Console.WriteLine(source.ToString() + " uses " + source.power.name + ".");
            Console.WriteLine(attEnergy.name + " attached to " + target.ToString());
            utils.Logger.Report(attEnergy.name + " attached to " + target.ToString());
            return true;
        }
        #endregion

        #region Trainers
        private bool ClefairyDoll(trainer source)
        {
            if (host.benched.Count >= 5) // Cannot be used
                return false;

            if (host.benched.Count > 1 && !host.benched.All(x=> x.damage + 10 == x.HP)) // Not worth to use
                return false;

            effects.CreateProxyBattler(host, source, source.parameters[0], source.parameters[1]);
            return true;
        }

        private bool ComputerSearch(trainer source)
        {
            if (host.hand.Count < 3) return false; // Cannot be executed

            Dictionary<card, int> scores = EvaluateCards(host.hand);

            List<card> discard = new List<card>();
            card temp;
            while( discard.Count != 2)
            {
                temp = scores.FirstOrDefault(x => x.Value <= scores.Values.Min()).Key;
                scores.Remove(temp);
                if (temp != source)
                    discard.Add(temp);
            }

            foreach (card ca in discard)
                host.CardToDiscard(ca);

            List<card> deckListed = host.deck.ToList();
            scores = EvaluateCards(deckListed);

            temp = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;

            host.deck.Remove(temp);
            host.hand.Add(temp);
            host.hand.Remove(source);

            return true;
        }

        private bool DevolutionSpray()
        {
            //TODO: This card is terrible and I don't want to do it
            return false;
        }

        private bool ImposterOak(trainer source, Player opp)
        {
            if (opp.hand.Count < 8) return false; // Not worth to do it

            opp.DiscardHand();
            opp.draw(7);
            host.hand.Remove(source);
            return true;
        }

        private bool ItemFinder(trainer source)
        {
            if (host.hand.Count < 3) return false; // Not enough cards
            if (!host.discarded.Any(x => x.getSuperType() == 2)) return false; // There isn't a trainer in the discard pile

            Dictionary<card, int> scores = EvaluateCards(host.hand);

            List<card> discard = new List<card>();
            card temp;
            while (discard.Count != 2)
            {
                temp = scores.FirstOrDefault(x => x.Value <= scores.Values.Min()).Key;
                scores.Remove(temp);
                if (temp != source)
                    discard.Add(temp);
            }

            foreach (card ca in discard)
                host.CardToDiscard(ca);

            scores = EvaluateCards(host.discarded.Where(x=> x.getSuperType() == 2).ToList());
            temp = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;
            discard.Remove(temp);
            host.hand.Add(temp);
            host.hand.Remove(source);
            return true;
        }

        private bool Lass(Player opp)
        {
            // TODO
            CheckModifications();
            if (opp.hand.Count / 3 - trainers.Count < 0) return false; // Not worth it

            effects.ShuffleCardsType(host, 2);
            effects.ShuffleCardsType(opp, 2);
            return true;
        }

        private bool PokemonTrader(trainer source)
        {
            if (!host.hand.Any(x => x.getSuperType() == 0)) return false; // No battler card in hand

            Dictionary<card, int> scoresDeck = EvaluateCards(host.deck.Where(x => x.getSuperType() == 0).ToList());
            if (!scoresDeck.Any()) return false; // No battler cards in the deck

            Dictionary<card, int> scoresHand = EvaluateCards(host.hand.Where(x => x.getSuperType() == 0).ToList());
            if (scoresDeck.Values.Min() >= scoresHand.Values.Max()) return false; // Not worth it

            card toDeck = scoresHand.FirstOrDefault(x => x.Value <= scoresHand.Values.Min()).Key;
            card toHand = scoresDeck.FirstOrDefault(x => x.Value >= scoresDeck.Values.Max()).Key;

            Console.WriteLine(host.ToString() + " uses " + source.name);
            Console.WriteLine(toHand.ToString() + " is put into " + host.ToString() + "'s hand.");
            Console.WriteLine(toDeck.ToString() + " is put into " + host.ToString() + "'s deck.");
            utils.Logger.Report(host.ToString() + " uses " + source.name);
            utils.Logger.Report(toHand.ToString() + " is put into " + host.ToString() + "'s hand.");
            utils.Logger.Report(toDeck.ToString() + " is put into " + host.ToString() + "'s deck.");

            host.deck.Remove(toHand);
            host.deck.AddFirst(toDeck);
            host.shuffle();

            host.hand.Remove(toDeck);
            host.hand.Add(toHand);

            host.CardToDiscard(source);

            return true;
        }

        private bool ScoopUp(trainer source, Player opp)
        {
            if (host.benched.Count == 1) return false; // Cannot execute
            battler selected = host.benched.FirstOrDefault(x => (x.HP - x.damage) < 30);
            if (selected == null) return false; // Not worth it

            Console.WriteLine(host.ToString() + " uses " + source.name);
            utils.Logger.Report(host.ToString() + " uses " + source.name);
            effects.ScoopUp(host, opp, selected);
            host.CardToDiscard(source);

            return true;
        }

        private bool SuperEnergyRemoval(trainer source, Player opp)
        {
            if (!opp.benched.Any(x => x.energies.Count >= 2)) return false; // No meeting condition
            if (!host.benched.Any(x => x.energies.Count >= 1)) return false; // No meeting condition


            // TODO: Reconsider this
            battler own = null;
            int actual;
            int best = int.MinValue;
            foreach (battler btl in host.benched)
            {
                actual = EvaluateAttachedEnergy(btl);
                if (actual > best)
                {
                    best = actual;
                    own = btl;
                }
            }

            battler other = null;
            best = int.MaxValue;
            foreach (battler btl in opp.benched)
            {
                actual = EvaluateAttachedEnergy(btl);
                if (actual < best && btl.energies.Count() >= 2)
                {
                    best = actual;
                    other = btl;
                }
            }

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.discardEnergy(own, WorstEnergy(own));
            opp.discardEnergy(other, BestEnergy(other));
            opp.discardEnergy(other, BestEnergy(other));

            host.CardToDiscard(source);

            return true;
        }

        private bool Defender(trainer source)
        {
            // TODO: Do some heuristic
            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            effects.addCondition(host.benched[0], Legacies.damageReduction, source.parameters);

            host.CardToDiscard(source);

            return true;
        }

        private bool EnergyRetrieval(trainer source)
        {
            if (host.hand.Count() < 2) return false; // Not enough cards in the hand to execute

            List<energy> disEnergy = host.discarded.Where(x => x is energy).Cast<energy>().ToList();
            if (disEnergy.Count(x=> x.type == 0) < 2 ) return false; // Not enough BASIC energy cards in the graveyard

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            Dictionary<card, int> scoresHand = EvaluateCards(host.hand.Where(x => x != source).ToList());
            host.CardToDiscard(scoresHand.FirstOrDefault(x => x.Value <= scoresHand.Values.Min()).Key);

            Dictionary<card, int> scores = EvaluateCards(disEnergy.Cast<card>().ToList());
            card rescued = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;
            scores.Remove(rescued);
            host.discarded.Remove(rescued);
            host.hand.Add(rescued);
            Console.WriteLine(rescued.ToString() + " moves from the discard pile to the hand.");
            utils.Logger.Report(rescued.ToString() + " moves from the discard pile to the hand.");

            rescued = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;
            scores.Remove(rescued);
            host.discarded.Remove(rescued);
            host.hand.Add(rescued);
            Console.WriteLine(rescued.ToString() + " moves from the discard pile to the hand.");
            utils.Logger.Report(rescued.ToString() + " moves from the discard pile to the hand.");

            host.CardToDiscard(source);

            return true;
        }

        private bool FullHeal(trainer source)
        {
            if (host.benched[0].status == 0) return false; // Already at full heal

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.benched[0].status = 0; // Annulate all status

            host.CardToDiscard(source);

            return true;
        }

        private bool Maintenance(trainer source)
        {
            if (host.hand.Count() < 3) return false; // Not enough cards to perform
            if (host.deck.Count() == 0) return false; // Not killing ourselves

            Dictionary<card, int> deckEvaluation = EvaluateCards(host.deck.ToList());
            int deckAverage = deckEvaluation.Values.Sum() / deckEvaluation.Count();

            Dictionary<card, int> handEvaluation = EvaluateCards(host.hand.Where(x => x != source).ToList());
            card t1 = handEvaluation.FirstOrDefault(x => x.Value <= handEvaluation.Values.Min()).Key;
            int evaluation = handEvaluation[t1];
            handEvaluation.Remove(t1);
            card t2 = handEvaluation.FirstOrDefault(x => x.Value <= handEvaluation.Values.Min()).Key;
            evaluation += handEvaluation[t2];

            if (evaluation >= deckAverage) return false; // Not worth to use it

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.deck.AddFirst(t1);
            host.deck.AddFirst(t2);

            Console.WriteLine(host.ToString() + " shuffles " + t1.ToString() + " and " + t2.ToString() + ".");
            utils.Logger.Report(host.ToString() + " shuffles " + t1.ToString() + " and " + t2.ToString() + ".");

            host.shuffle();

            host.draw(1);

            host.CardToDiscard(source);

            return true;
        }

        private bool PlusPower(trainer source)
        {
            // TODO: Include an heuristic for this effect
            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            effects.addCondition(host.benched[0], Legacies.damageAmplification, source.parameters);

            host.CardToDiscard(source);

            return true;
        }

        private bool PokemonCenter(trainer source)
        {
            if (energies.Count() < 2) return false; // Usually not worth it

            int damageSum = 0;
            int energySum = 0;
            foreach (battler btl in host.benched)
                if (btl.damage != 0)
                {
                    damageSum += btl.damage;
                    energySum += btl.energies.Count();
                }

            energySum *= 20;

            if (energySum > damageSum) return false; // Not a tradeoff

            effects.HealAndDiscardEnergy(host);

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.CardToDiscard(source);

            return true;

        }

        private bool PokemonFlute(trainer source, Player opp)
        {
            if (opp.benched.Count() == 1) return false; // No favours

            List<battler> targets = opp.discarded.Where(x => x is battler).Cast<battler>().ToList();
            if (!targets.Any()) return false; // No legal targets (generally discarded battlers means discarded basics)

            targets = targets.Where(x => x.type == 0).ToList(); // Look for basics
            if (!targets.Any()) return false; // Still no legal targets

            // TODO: Include more heuristics
            battler target = targets.FirstOrDefault(x => x.HP <= 30 || (x.HP <= 40 && x.weak_elem == host.benched[0].element));
            if (target == null) return false; // No good target

            opp.discarded.Remove(target);
            opp.benched.Add(target);

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            Console.WriteLine(target.ToString() + " is put in play from the discard pile.");
            utils.Logger.Report(target.ToString() + " is put in play from the discard pile.");

            host.CardToDiscard(source);

            return true;
        }

        private bool Pokedex(trainer source)
        {
            // Always execute
            List<card> scry = new List<card>();

            for (int i = 0; i < 5; i++)
            {
                scry.Add(host.deck.First.Value);
                host.deck.RemoveFirst();
            }

            Dictionary<card, int> scores = EvaluateCards(scry);

            for (int i = 0; i < 5; i++)
            {
                host.deck.AddFirst(scores.First(x => x.Value <= scores.Values.Min()).Key);
                scores.Remove(host.deck.First.Value);
            }

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.CardToDiscard(source);

            return true;
        }

        private bool ProfesorOak(trainer source)
        {
            if (host.hand.Count() > 3) return false; // Not even think about it
            Dictionary<card, int> deckEvaluation = EvaluateCards(host.deck.ToList());
            int deckAverage = deckEvaluation.Values.Sum() / deckEvaluation.Count() * 7;

            Dictionary<card, int> handEvaluation = EvaluateCards(host.hand.Where(x => x != source).ToList());
            int handAverage = handEvaluation.Values.Sum();

            if (deckAverage < handAverage) return false; // Not worth it, but not very probable

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.DiscardHand();
            host.draw(7);

            host.CardToDiscard(source);

            return true;
        }

        private bool Revive(trainer source)
        {
            if (host.benched.Count() == 6) return false; // Bench full

            // This huge line selects all the basic battlers from the discard pile
            List<battler> targets = host.discarded.Where(x => x is battler).Cast<battler>().Where(x => x.type == 0).ToList();

            if (!targets.Any()) return false; // No possible target

            Dictionary<battler, int> scores = EvaluateBasic(targets);

            int threshold = host.benched.Count == 1 ? -2 : 0;
            battler selected = scores.FirstOrDefault(x => x.Value == scores.Values.Max() && x.Value > threshold).Key;

            if (selected == null) return false; // No good enough

            host.discarded.Remove(selected);
            host.benched.Add(selected);
            selected.damage = selected.HP % 2 == 5 ? selected.HP / 2 - 5 : selected.HP / 2;

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.CardToDiscard(source);

            return true;
        }

        private bool SuperPotion(trainer source)
        {
            List<battler> targets = host.benched.Where(x => x.damage >= 50 && x.energies.Count >= 1).ToList();
            if (!targets.Any()) return false; // No good targets

            battler target = null;
            int best = int.MinValue;
            int actual;
            foreach (battler btl in targets)
            {
                actual = EvaluateAttachedEnergy(btl) + btl.damage / 10;
                if (btl == host.benched[0])
                    actual += 1; // Little bump for being front

                if ( actual > best )
                {
                    best = actual;
                    target = btl;
                }
            }

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.discardEnergy(target, WorstEnergy(target));
            effects.heal(target, 50);

            host.CardToDiscard(source);

            return true;
        }

        private bool Bill(trainer source)
        {
            if (host.deck.Count < 2) return false; // The only restrictive execution condition

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.draw(2);

            host.CardToDiscard(source);

            return true;
        }

        private bool EnergyRemoval(trainer source, Player opp)
        {
            if (!opp.benched.Any(x => x.energies.Count >= 1)) return false; // No meeting condition

            // TODO: Reconsider it
            battler other = null;
            int best = int.MaxValue;
            int actual;
            foreach (battler btl in opp.benched)
            {
                actual = EvaluateAttachedEnergy(btl);
                if (actual < best && btl.energies.Count() >= 1)
                {
                    best = actual;
                    other = btl;
                }
            }

            if (other == null) return false; // No one meets conditions

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            opp.discardEnergy(other, BestEnergy(other));

            host.CardToDiscard(source);

            return true;
        }

        private bool GustOfWind(trainer source, Player opp)
        {
            if (opp.benched.Count < 2) return false; // Cannot be used

            Dictionary<battler, int> scores = EvaluateActive(opp.benched);

            int frontScore = scores[opp.benched[0]]; // Remove front
            scores.Remove(opp.benched[0]);

            if (!scores.Any(x => x.Value < frontScore)) return false; // Not worth it

            battler selected = scores.FirstOrDefault(x => x.Value <= scores.Values.Min()).Key;

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            opp.ExchangePosition(selected);

            host.CardToDiscard(source);

            return true;
        }

        private bool Potion(trainer source)
        {
            List<battler> targets = host.benched.Where(x => x.damage >= 20).ToList();
            if (!targets.Any()) return false; // No targets

            battler target = null;
            if (targets.Contains(host.benched[0])) // Priority to front
                target = host.benched[0];
            else
                target = targets.First();


            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            effects.heal(target, 20);

            host.CardToDiscard(source);

            return true;
        }

        private bool Switch(trainer source)
        {
            if (host.benched.Count < 2) return false; // Not usable

            Dictionary<battler, int> scores = EvaluateActive(host.benched);

            int frontScore = scores[host.benched[0]]; // Remove front
            scores.Remove(host.benched[0]);

            if (!scores.Any(x => x.Value > frontScore)) return false; // Not worth it

            battler selected = scores.FirstOrDefault(x => x.Value >= scores.Values.Max()).Key;

            Console.WriteLine(host.ToString() + " uses " + source.ToString() + ".");
            utils.Logger.Report(host.ToString() + " uses " + source.ToString() + ".");

            host.ExchangePosition(selected);

            host.CardToDiscard(source);

            return true;
        }
        #endregion

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
