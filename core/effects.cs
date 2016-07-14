using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    class effects
    {
        public static void move_selector(Player source_controller, Player target_controller, battler source, battler target, int type, int selector, int quantity1, int quantity2)
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
                    if (CRandom.RandomInt() < 0)
                        inflictStatus(target, quantity2);
                    damage(type, quantity1, target);
                    break;
                case 3: // Add condition by coin
                    if (CRandom.RandomInt() < 0)
                        addCondition(source, quantity1, quantity2);
                    break;
            }
        }

        public static void damage(int type, int quantity, battler target)
        {
            if (target.conditions.ContainsKey(Legacies.fog))
            {
                Console.WriteLine(target.ToString() + " is protected from damage.");
                return;
            }
            int output = quantity;
            if (type == target.weak_elem) output *= target.weak_mod;
            else if (type == target.res_elem) output -= target.res_mod;

            target.damage += output;

            Console.WriteLine(target.ToString() + " received " + output + " points of damage.");
        }

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
                    if (source.energies[digit].elem == type)
                    {
                        source_controller.discardEnergy(source, digit);
                        end = true;
                    }

                }
            }
            Console.WriteLine("Energy succcessfully discarded");
        }

        public static void heal(battler source, int quantity)
        {
            if (quantity == 0)
            {
                source.damage = 0;
                Console.WriteLine("Damage eliminated");
            }
            else
            {
                source.damage -= quantity;
                Console.WriteLine("Removed "+quantity+" damage");
            }
        }

        public static void inflictStatus(battler target, int type)
        {
            target.status = type;
            Console.WriteLine(target.ToString() + " is now " + utilities.numToStatus(type));
        }

        public static void addCondition(battler source, int condition, int duration)
        {
            source.conditions.Add(condition, duration);
            Console.WriteLine("Condition activated");
        }
    }
}
