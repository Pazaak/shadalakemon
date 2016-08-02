using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class effects
    {
        // A big-old switch which contains the necessary calls to execute movements 
        public static void move_selector(Player source_controller, Player target_controller, battler source, battler target, movement mov, int selector, int[] parameters, bool costless)
        {
            switch (selector)
            {
                case 0: // Simple damage
                    damage(source.element, parameters[0], target, source, target_controller, source_controller); 
                    break;
                case 1: // Discard and heal
                    if ( !costless )
                        discardEnergy(source_controller, source, source.element, parameters[0]);
                    heal(source, parameters[1]);
                    break;
                case 2: // Damage and coin for status
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        inflictStatus(target, parameters[1]);
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 3: // Add condition by coin
                    int[] temp;
                    if (CRandom.RandomInt() < 0)
                    {
                        temp = new int[parameters.Length-1];
                        Array.Copy(parameters, 1, temp, 0, temp.Length);
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        addCondition(source, parameters[0], temp); 
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 4: // Damage empowered by excess of energy
                    int exEner = ExcessEnergy(mov, source, parameters[2], costless);
                    damage(source.element, parameters[0] + (exEner > parameters[1] ? parameters[1] : exEner), target, source, target_controller, source_controller);
                    break;
                case 5: // Deactivate a movement
                    MoveDeactivator(target);
                    break;
                case 6: // Flip Q2 coins, do goodFlips(Q2)*Q1 damage
                    damage(source.element, FlipDamage(parameters[0], parameters[1]), target, source, target_controller, source_controller);
                    break;
                case 7: // Damage and discard
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    discardEnergy(target_controller, target, -1, 1);
                    break;
                case 8: // Damage equal the number of damage counters
                    damage(source.element, source.damage, target, source, target_controller, source_controller);
                    break;
                case 9: // Damage and wheel
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    Wheel(target_controller);
                    break;
                case 10: // Damage = Half of the remaining HP
                    int result = (target.HP - target.damage) / 2;
                    result += result % 10 == 5 ? 5 : 0;
                    damage(source.element, result, target, source, target_controller, source_controller);
                    break;
                case 11: // Change target weakness
                    ChangeWeakness(target);
                    break;
                case 12: // Change own resistance
                    ChangeResistance(source);
                    break;
                case 13: // Leek Slap!
                    damage(source.element, FlipDamage(30, 1), target, source, target_controller, source_controller);
                    source.leekSlap = true;
                    break;
                case 14: // Base set mirror movement
                    MirrorMove(source_controller, source, target_controller, target, parameters[0]);
                    break;
                case 15: // Execute a movement of the defender pokemon without paying costs
                    ExDefMovement(source_controller, target_controller, source, target);
                    break;
                case 16: // Status by coin
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        inflictStatus(target, parameters[0]);
                    }
                    else
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                    break;
                case 17: // Damage at both sides
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    damage(Constants.TNone, parameters[1], source, source, target_controller, source_controller);
                    break;
                case 18: // Discard to do damage
                    if (!costless)
                        discardEnergy(source_controller, source, parameters[1], parameters[2]);
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    break;
                case 19: // Wheel
                    Wheel(target_controller);
                    break;
                case 20: // Damage and status
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    inflictStatus(target, parameters[1]);
                    break;
                case 21: // Damage and one of two status by coin
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        inflictStatus(target, parameters[1]);
                    }
                    else
                    {
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                        inflictStatus(target, parameters[2]);
                    }
                    break;
                case 22: // Damage and leech
                    if (damage(source.element, parameters[0], target, source, target_controller, source_controller) > 0)
                        heal(source, parameters[1]);
                    break;
                case 23: // Extra damage or recoil
                    bool extraDamage;
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report(source_controller.ToString() + " wins the coin flip.");
                        extraDamage = true;
                    }
                    else
                    {
                        utils.Logger.Report(source_controller.ToString() + " loses the coin flip.");
                        extraDamage = false;
                    }
                    damage(source.element, extraDamage? parameters[0] + parameters[1] : parameters[0], target, source, target_controller, source_controller);
                    if (!extraDamage) damage(Constants.TNone, parameters[2], source, source, target_controller, source_controller);
                    break;
                case 24: // Discard type and legacy [element, quantity, legacy, duration]
                    if (!costless)
                        discardEnergy(source_controller, source, parameters[0], parameters[1]);
                    temp = new int[parameters.Length - 3];
                    Array.Copy(parameters, 3, temp, 0, temp.Length);
                    addCondition(source, parameters[2], temp);
                    break;
                case 25: // Damage + opposite damage counters
                    damage(source.element, parameters[0] + target.damage, target, source, target_controller, source_controller);
                    break;
                case 26: // Status
                    inflictStatus(target, parameters[0]);
                    break;
                case 27: // Damage only while statused
                    if (target.status == parameters[1])
                        damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    else
                        Console.WriteLine(target.ToString() + " is not under " + utilities.numToStatus(parameters[1]));
                    break;
                case 28: // Damage empowered by the energy attached to the target
                    damage(source.element, parameters[0] + target.energies.Count*10, target, source, target_controller, source_controller);
                    break;
                case 29: // Damage and legacy to the opponent
                    damage(source.element, parameters[0], target, source, target_controller, source_controller);
                    temp = new int[parameters.Length - 2];
                    Array.Copy(parameters, 2, temp, 0, temp.Length);
                    addCondition(target, parameters[1], temp);
                    break;
            }
        }

        // A big-old which that indicates the effects of the powers
        public static void power_selector(Player source_controller, battler source, int selector, int[] parameters)
        {
            switch (selector)
            {
                case 0: // Rain dance
                    PlayFreeEnergy(source_controller, parameters[0]);
                    // Can be executed any number of times
                    break;
                case 1: // Energy trans
                    SwitchEnergySameType(source_controller, parameters[0]);
                    break;
                case 2: // Damage Swap
                    ExchangeDamage(source_controller);
                    break;
            }
        }

        // Does plain damage taking into account conditions and type effectiveness
        public static int damage(int type, int quantity, battler target, battler source, Player target_controller, Player source_controller)
        {
            if (target.conditions.ContainsKey(Legacies.fog)) // Prevent damage condition
            {
                Console.WriteLine(target.ToString() + " is protected from damage.");
                utils.Logger.Report(target.ToString() + " is protected from damage.");
                return 0;
            }
            int output = quantity;
            if (type == target.weak_elem) output *= target.weak_mod; // Apply weaknesses
            else if (type == target.res_elem) output -= target.res_mod; // Apply resistances

            if (output > 0)
                target.damage += output;
            else
                output = 0;

            Console.WriteLine(target.ToString() + " received " + output + " points of damage.");
            utils.Logger.Report(target.ToString() + " received " + output + " points of damage.");

            // Check for knockout
            if (target.damage >= target.HP)
            {
                bool doomIndicator = false;
                if (source != null && target.conditions.ContainsKey(Legacies.destinyBound))
                    doomIndicator = true;

                target_controller.frontToDiscard();

                source_controller.PriceProcedure();

                if (source_controller.winCondition) return output; // break

                if (target_controller.benched.Count > 0)
                    target_controller.KnockoutProcedure();
                else
                {
                    Console.WriteLine(target_controller.ToString() + " has no benched pokemon. " + source_controller.ToString() + " wins."); // The player losses
                    utils.Logger.Report(target_controller.ToString() + " has no benched pokemon. " + source_controller.ToString() + " wins.");
                    source_controller.winCondition = true;
                }

                if (doomIndicator)
                    effects.damage(Constants.TNone, source.HP - source.damage, source, null, source_controller, target_controller);
            }

            return output;
        }

        // Effect to discard a card, as a cost or as an effect
        public static void discardEnergy(Player source_controller, battler source, int type, int quantity)
        {
            bool end;
            int digit;
            if (source.energies.Count == 0)
                Console.WriteLine(source.ToString() + " has no energy to discard.");

            for ( int i = 0; i < quantity; i++)
            {
                if (source.energies.Count == 0)
                {
                    Console.WriteLine(source.ToString() + " has no more energy to discard.");
                    Console.WriteLine("Energy succcessfully discarded");
                    return;
                }
                
                Console.WriteLine("Select to discard " + utilities.numToType(type) + " energy card/s. " + (i + 1) + "/" + quantity);
                end = false;
                while (!end)
                {
                    Console.WriteLine(source.showEnergy());
                    digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
                    if (source.energies[digit].elem == type || type == -1 || (type == Constants.TFire && source.conditions.ContainsKey(Legacies.energyBurn)))
                    {
                        source_controller.discardEnergy(source, digit);
                        end = true;
                    }

                }
            }
            Console.WriteLine("Energy succcessfully discarded");
        }

        // Effect to heal the source battler, 0 for full heal
        public static void heal(battler source, int quantity)
        {
            if (quantity == 0)
            {
                source.damage = 0;
                Console.WriteLine("Damage eliminated");
                utils.Logger.Report(source.ToString() + " has its damage removed.");
            }
            else
            {
                source.damage -= source.damage >= quantity? quantity : 0;
                Console.WriteLine(source.ToString()+" has "+quantity+" damage removed.");
                utils.Logger.Report(source.ToString() + " has " + quantity + " damage removed.");
            }
        }

        // Assign an status alignment
        public static void inflictStatus(battler target, int type)
        {
            target.status = type;
            Console.WriteLine(target.ToString() + " is now " + utilities.numToStatus(type));
            utils.Logger.Report(target.ToString() + " is now " + utilities.numToStatus(type));

        }

        // Adds a condition
        public static void addCondition(battler source, int condition, int[] parameters)
        {
            source.conditions.Add(condition, parameters);
            Console.WriteLine(source.ToString()+Legacies.IndexToLegacy(condition));
            utils.Logger.Report(source.ToString() + Legacies.IndexToLegacy(condition));
        }

        // Permits to exchange energy attached to one battler to other
        public static void SwitchEnergySameType(Player source_controller, int elem)
        {
            Console.WriteLine("Select a Pokemon with an energy card of the selected type");
            source_controller.DisplayTypedEnergies(elem);

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;
            battler source = null;

            if (digit == -1)
                source = source_controller.front;
            else
                source = source_controller.benched[digit];

            if (source.energies.Count == 0)
            {
                Console.WriteLine("That pokemon has not enough energy cards");
                return;
            }

            int counter = 0;
            while (source.energies[counter].elem != elem)
                counter++;

            energy selected = source.energies[counter];
            source.energies.RemoveAt(counter);

            Console.WriteLine("Select a Pokemon to attach the energy");
            source_controller.DisplayTypedEnergies(elem);

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

            battler target = null;

            if (digit == -1)
                target = source_controller.front;
            else 
                target = source_controller.benched[digit];

            target.attachEnergy(selected);

            Console.WriteLine(selected.name + " attached from " + source.ToString() + " to " + target.ToString());
            utils.Logger.Report(selected.name + " attached from " + source.ToString() + " to " + target.ToString());

        }

        // Checks for excesses of energy for a determined movement and adds it to the power
        public static int ExcessEnergy(movement mov, battler source, int type, bool costless)
        {
            if (costless) return source.energyTotal[type]*10;

            int excess = source.energyTotal[type] - mov.cost[type];

            if (excess <= 0) return 0;

            int colorless = 0;

            for (int i = 0; i < mov.cost.Length; i++)
                if (i != type)
                    colorless += source.energyTotal[i];

            if (colorless >= mov.cost[0]) return excess * 10;

            return (excess + colorless - mov.cost[0])*10;
        }

        // Deactivates a movement
        public static void MoveDeactivator(battler target)
        {
            Console.WriteLine("Select the movement to deactivate:");
            for (int i = 0; i < target.movements.Length; i++)
                Console.WriteLine((i + 1) +"- "+ target.movements[i].ToString());

            addCondition(target, Legacies.deacMov, new int[2] { 2, Convert.ToInt16(Console.ReadKey().KeyChar) - 49 });
        }

        // Deals damage equals to the number of flips winned
        public static int FlipDamage(int quantity, int flips)
        {
            int multiplier = 0;

            for (int i = 0; i < flips; i++)
                if (CRandom.RandomInt() < 0)
                    multiplier++;

            Console.WriteLine(multiplier + " successful flips.");
            utils.Logger.Report(multiplier + " successful flips.");
            return quantity * multiplier;
        }

        // Wheels the front pokemon
        public static void Wheel(Player target)
        {
            if ( target.benched.Count == 0)
            {
                Console.WriteLine("There are no pokemon in the bench.");
                return;
            }

            Console.WriteLine(target.ToString() + "- Select the pokemon to exchange:");
            Console.WriteLine(target.writeBenched());
            target.ExchangePosition(Convert.ToInt16(Console.ReadKey().KeyChar) - 49);
            Console.WriteLine("Pokemon exchanged.");
        }

        // Change weakness of target battler
        public static void ChangeWeakness(battler target)
        {
            if ( target.weak_elem == Constants.TNone )
            {
                Console.WriteLine("Defending Pokemon has no weakness");
                utils.Logger.Report("Defending Pokemon has no weakness");
                return;
            }
            Console.WriteLine("Select the new type for the defending weakness: ");
            Console.WriteLine("1 - Water" + Environment.NewLine + "2 - Fire" + Environment.NewLine + "3 - Grass"
                + Environment.NewLine + "4 - Psychic" + Environment.NewLine + "5 - Fighting" + Environment.NewLine + "6 - Lightning");
            target.weak_elem = Convert.ToInt16(Console.ReadKey().KeyChar) - 48;

            Console.WriteLine(target.ToString() + " is now weak to " + utilities.numToType(target.weak_elem) + ".");
            utils.Logger.Report(target.ToString() + " is now weak to " + utilities.numToType(target.weak_elem) + ".");
        }

        // Change resistance of target battler
        public static void ChangeResistance(battler target)
        {
            Console.WriteLine("Select the new type for the defending resistance: ");
            Console.WriteLine("1 - Water" + Environment.NewLine + "2 - Fire" + Environment.NewLine + "3 - Grass"
                + Environment.NewLine + "4 - Psychic" + Environment.NewLine + "5 - Fighting" + Environment.NewLine + "6 - Lightning");
            target.res_elem = Convert.ToInt16(Console.ReadKey().KeyChar) - 48;

            Console.WriteLine(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
            utils.Logger.Report(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
        }

        // Mirror move
        public static void MirrorMove(Player source_controller, battler source, Player target_controller, battler target, int parameter)
        {
            if ( source_controller.lastFront != source_controller.front )
            {
                Console.WriteLine(source_controller.front.ToString() + " wasn't attacked last turn.");
                utils.Logger.Report(source_controller.front.ToString() + " wasn't attacked last turn.");
                return;
            }

            // Damage phase
            string[] log = utils.Logger.FindCombatString(target_controller.ToString(), source_controller.ToString());

            foreach (string s in log)
            {
                if ( s.Contains(source.ToString() + " received ") )
                {
                    string[] temp = s.Split(' ');
                    damage(Constants.TNone, Int32.Parse(temp[2]), target, source, target_controller, source_controller);
                }
            }

            if ( parameter == 1 ) // All the other effects (base set mirror move)
            {
                foreach (string s in log)
                {
                    if (s.Contains(source.ToString() + " had its energy "))
                        discardEnergy(target_controller, target, -1, 1);
                    if (s.Contains(source.ToString() + " is now "))
                    {
                        string[] temp = s.Split(' ');
                        inflictStatus(target, utilities.nameToStatus(temp[3]));
                    }
                    if (s.Contains(" attack deactivated.") && s.Contains(source.ToString()))
                        MoveDeactivator(target);
                    if (s.Contains(source.ToString()+" is now weak to "))
                        ChangeWeakness(target);
                }
            }
        }

        // Execute a movement of the defending pokemon
        public static void ExDefMovement(Player source_controller, Player target_controller, battler source, battler target)
        {
            Console.WriteLine(target.BattleDescription());
            Console.WriteLine("Select a movement: ");
            movement mov = target.movements[Convert.ToInt16(Console.ReadKey().KeyChar) - 49];
            mov.execute(source_controller, target_controller, source, target, true);
        }

        public static void PlayFreeEnergy(Player source, int elem)
        {
            Console.WriteLine("Attach " + utilities.numToType(elem) + " a energy card from your hand to " + utilities.numToType(elem) + " pokémon. 0 to exit");
            Console.WriteLine(source.writeHand());

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;

            if (digit == -1) // Exit without selecting
                return;

            if ( source.hand[digit].getSuperType() != 2) // Not an energy card
            {
                Console.WriteLine("That isn't a energy card.");
                return;
            }

            energy selected = (energy)source.hand[digit];

            if ( selected.elem != elem)
            {
                Console.WriteLine("That isn't a "+ utilities.numToType(elem)+" energy card.");
                return;
            }

            Console.WriteLine("Select a " + utilities.numToType(elem) + " pokémon. 0 to exit");
            Console.WriteLine(source.writeBattlers());

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

            if (digit == -2) // Exit without selecting
                return;

            battler target = null;

            if (digit == -1) target = source.front;
            else target = source.benched[digit];

            if (target.element != elem) // Not a pokémon of the selected type
            {
                Console.WriteLine("That isn't a "+utilities.numToType(elem)+" pokémon.");
                return;
            }

            target.attachEnergy(selected);
            source.hand.Remove(selected);
        }

        // Exchange damage between battlers
        public static void ExchangeDamage(Player source_controller)
        {
            Console.WriteLine("Select a pokemon with damage counters on it: (Press 0 to exit)");
            Console.WriteLine(source_controller.ShowDamage());

            int digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50; // selection
            if (digit == -2) return; // 0 equals exit

            battler source; // select the source of the damage
            if (digit == -1) source = source_controller.front;
            else source = source_controller.benched[digit];

            if (source.damage == 0) // Discard a selection with zero damage
            {
                Console.WriteLine("That pokemon hasn't damage counters on it");
                return;
            }

            source.damage -= 10; // Substract the damage from source

            Console.WriteLine("Select a pokemon to put damage counters on it: (Press 0 to exit)");
            Console.WriteLine(source_controller.ShowDamage());

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50; // selection
            if (digit == -2) // 0 equals exit
            {
                source.damage += 10;
                return;
            } 

            battler target; // select the source of the damage
            if (digit == -1) target = source_controller.front;
            else target = source_controller.benched[digit];

            if (target.damage+10 == target.HP) // Discard a selection that will be knocked out
            {
                source.damage += 10;
                Console.WriteLine("That pokemon will be knocked out with further damage");
                return;
            }

            // All clear, proceed to exchange damage
            target.damage += 10;
            Console.WriteLine(source.ToString() + " losses a damage counter. " + target.ToString() + " obtains a damage counter");
            utils.Logger.Report(source.ToString() + " losses a damage counter. " + target.ToString() + " obtains a damage counter");
        }
    }
}
