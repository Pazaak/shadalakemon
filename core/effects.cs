using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class effects
    {
        // A big-old switch which contains the necessary calls to execute movements 
        public static void move_selector(Player source_controller, Player target_controller, battler source, battler target, movement mov, int type, int selector, int quantity1, int quantity2)
        {
            switch (selector)
            {
                case 0: // Simple damage
                    damage(type, quantity1, target); 
                    break;
                case 1: // Discard and heal
                    discardEnergy(source_controller, source, type, quantity1);
                    heal(source, quantity2);
                    break;
                case 2: // Damage and coin for status
                    damage(type, quantity1, target);
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report("You win the coin flip.");
                        inflictStatus(target, quantity2);
                    }
                    else
                        utils.Logger.Report("You lose the coin flip.");
                    break;
                case 3: // Add condition by coin
                    if (CRandom.RandomInt() < 0)
                    {
                        utils.Logger.Report("You win the coin flip.");
                        addCondition(source, quantity1, quantity2); 
                    }
                    else
                        utils.Logger.Report("You lose the coin flip.");
                    break;
                case 4: // Damage empowered by excess of energy
                    quantity1 += ExcessEnergy(mov, source) > quantity2? quantity2 : ExcessEnergy(mov, source);
                    damage(type, quantity1, target);
                    break;
                case 5: // Deactivate a movement
                    MoveDeactivator(target);
                    break;
                case 6: // Flip Q2 coins, do goodFlips(Q2)*Q1 damage
                    FlipDamage(target, type, quantity1, quantity2);
                    break;
                case 7: // Damage and discard
                    damage(type, quantity1, target);
                    discardEnergy(target_controller, target, -1, 1);
                    break;
                case 8: // Damage equal the number of damage counters
                    damage(type, source.damage, target);
                    break;
                case 9: // Damage and wheel
                    damage(type, quantity1, target);
                    Wheel(target_controller);
                    break;
                case 10: // Damage = Half of the remaining HP
                    int result = (target.HP - target.damage) / 2;
                    result += result % 10 == 5 ? 5 : 0;
                    damage(type, result, target);
                    break;
                case 11: // Change target weakness
                    ChangeWeakness(target);
                    break;
                case 12: // Change own resistance
                    ChangeResistance(source);
                    break;
                case 13: // Leek Slap!
                    FlipDamage(target, type, quantity1, quantity2);
                    source.leekSlap = true;
                    break;
            }
        }

        // A big-old which that indicates the effects of the powers
        public static void power_selector(Player source_controller, battler source, int selector, int quantity1, int quantity2)
        {
            switch (selector)
            {
                case 0: // Rain dance
                    SwitchEnergySameType(source_controller, quantity1);
                    // Can be executed any number of times
                    break;
            }
        }

        // Does plain damage taking into account conditions and type effectiveness
        public static void damage(int type, int quantity, battler target)
        {
            if (target.conditions.ContainsKey(Legacies.fog)) // Prevent damage condition
            {
                Console.WriteLine(target.ToString() + " is protected from damage.");
                utils.Logger.Report(target.ToString() + " is protected from damage.");
                return;
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
        }

        // Effect to discard a card, as a cost or as an effect
        public static void discardEnergy(Player source_controller, battler source, int type, int quantity)
        {
            bool end;
            int digit;
            for ( int i = 0; i < quantity; i++)
            {
                Console.WriteLine("Select to discard " + utilities.numToType(type) + " energy card/s. " + (i + 1) + "/" + quantity);
                end = false;
                while (!end)
                {
                    Console.WriteLine(source.showEnergy());
                    digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 49;
                    if (source.energies[digit].elem == type || type == -1)
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
                source.damage -= quantity;
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
        public static void addCondition(battler source, int condition, int duration)
        {
            source.conditions.Add(condition, duration);
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
            while (!source.energies[counter].name.Equals("Water Energy")) // TODO: Only can take Water Energy
                counter++;

            energy selected = source.energies[counter];
            source.energies.RemoveAt(counter);

            Console.WriteLine("Select a Pokemon to attach the energy");
            source_controller.DisplayTypedEnergies(elem);

            digit = Convert.ToInt16(Console.ReadKey().KeyChar) - 50;

            battler target = null;

            if (digit == -1 && source_controller.front.element == elem)
                target = source_controller.front;
            else if (source_controller.benched[digit].element == elem)
                target = source_controller.benched[digit];
            else
            {
                Console.WriteLine("This pokemon is not " + utilities.numToType(elem) + " type.");
                return;
            }

            target.attachEnergy(selected);

            Console.WriteLine(selected.name + " attached from " + source.ToString() + " to " + target.ToString());
            utils.Logger.Report(selected.name + " attached from " + source.ToString() + " to " + target.ToString());

        }

        // Checks for excesses of energy for a determined movement and adds it to the power
        public static int ExcessEnergy(movement mov, battler source)
        {
            int excess = source.energyTotal[mov.type] - mov.cost[mov.type];

            if (excess <= 0) return 0;

            int colorless = 0;

            for (int i = 0; i < mov.cost.Length; i++)
                if (i != mov.type)
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

            addCondition(target, Convert.ToInt16(Console.ReadKey().KeyChar) - 48, 2);
        }

        // Deals damage equals to the number of flips winned
        public static void FlipDamage(battler target, int type, int quantity, int flips)
        {
            int multiplier = 0;

            for (int i = 0; i < flips; i++)
                if (CRandom.RandomInt() < 0)
                    multiplier++;

            Console.WriteLine(multiplier + " successful flips.");
            utils.Logger.Report(multiplier + " successful flips.");
            damage(type, quantity * multiplier, target);
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
            if ( target.weak_mod == 0 )
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
            Console.WriteLine("Select the new type for the defending weakness: ");
            Console.WriteLine("1 - Water" + Environment.NewLine + "2 - Fire" + Environment.NewLine + "3 - Grass"
                + Environment.NewLine + "4 - Psychic" + Environment.NewLine + "5 - Fighting" + Environment.NewLine + "6 - Lightning");
            target.res_elem = Convert.ToInt16(Console.ReadKey().KeyChar) - 48;

            Console.WriteLine(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
            utils.Logger.Report(target.ToString() + " is now resistant to " + utilities.numToType(target.res_elem) + ".");
        }
    }
}
