﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    /*
     * BATTLER CLASS
     * Holds data of the pokemon cards. It is a realization of class card, in order to be able to hold them in hand with other types of card.
     * Attributes:
     *  Type- Determines which type of pokemon card it is:
     *      0- Basic pokemon
     *      1- Evolution pokemon
     *  Element- Determines of which type the pokemon is:
     *      0- Normal
     *      1- Water
     *      2- Fire
     *      3- Grass
     *      4- Psychic
     *      5- Fighting
     *      6- Lightning
     *  HP- The hit points of the pokemon card
     *  weak_elem- Indicates to which type the pokemon is weak
     *  weak_mod- Indicates by how much the damage is multiplied
     *  res_elem- Indicates to which type the pokemon is resistant
     *  res_mod- Indicates by how much the damage is reduced
     *  retreat- Indicates the cost of retreat
     *  damage- Indicates the amount of damage in multiples of ten
     *  status- Indicates which status the battler is suffering
     *      0- No status
     *      1- Paralyzed
     *      2- Asleep
     *      3- Confused
     *  id- Indicates the number of the pokemon in the national dex (for evolutive purposes)
     *  evolvesFrom- Indicates the number of the previous pokemon of the evolutive chain in the national dex
     *  name- Name of the pokemon
     *  movements- A list of 'movement' objects
     *  energies- A list of 'energy' objects (cards)
     *  energyTotal- A list of integers that keeps track of the available energy of the battler
     *  sumSick- Indicates if the pokemon was cast on the actual turn
     *  prevolutions- A list that hold all the pokemon cards previous to the current one
     *  conditions- Holds a list of the conditions that the pokemon has
     *  power- Holds an object which contains the pokemon power if any
     *  originalWeakElem, originalResElem- Holds the original elements printed on the card (the other variables are subject to changes)
     *  leekSlap- The VERY odd attack of farfetch'd has a boolean on each battler to control it
     */
    public class battler : card
    {
        public int type, element, HP, weak_elem, weak_mod, res_elem, res_mod, retreat, damage, status, id, evolvesFrom;
        public int originalWeakElem, originalResElem;
        public string name;
        public movement[] movements;
        public List<energy> energies;
        public int[] energyTotal;
        public bool sumSick, leekSlap;
        public LinkedList<battler> prevolutions;
        public Dictionary<int, int> conditions;
        public Power power;

        public battler(int type, int element, int HP, int weak_elem, int weak_mod, int res_elem, int res_mod, int retreat, string name, int id, int evolvesFrom, movement[] movements, Power power)
        {
            this.type = type;
            this.element = element;
            this.HP = HP;
            this.weak_elem = weak_elem;
            this.originalWeakElem = weak_elem;
            this.weak_mod = weak_mod;
            this.res_elem = res_elem;
            this.originalResElem = res_elem;
            this.res_mod = res_mod;
            this.retreat = retreat;
            this.name = name;
            this.id = id;
            this.evolvesFrom = evolvesFrom;
            this.movements = movements;
            this.power = power;
            // Initilization of the internal variables
            this.damage = 0;
            this.status = 0;
            this.sumSick = true;
            energies = new List<energy>();
            energyTotal = new int[7];
            this.prevolutions = new LinkedList<battler>();
            this.conditions = new Dictionary<int, int>();
            
        }

        // Sends the execute instruction to the selected movement
        // index- The index of the movement that must be used
        // source_controller- The controller of the battler that executes the movement
        // target_controller- The controller of the target of the movement
        // target- The target of the movement
        public void execute(int index, Player source_controller, Player target_controller, battler target)
        {
            Console.WriteLine();
            movement selected = movements[index]; // Finds the movement

            if (conditions.ContainsKey(index+1))
            {
                Console.WriteLine("Movement "+(index+1)+" is deactivated!");
                return;
            }

            if (leekSlap && index == 0)
            {
                Console.WriteLine("YOU WILL NOT USE LEEK SLAP TWICE BY THE GRACE OF LORD FARFETCH'D");
                return;
            }

            if (selected.usable) // Checks if it's usable
                selected.execute(source_controller, target_controller, this, target); // Execute the selected movement
            else
                Console.WriteLine("Not enough energy to use that"); 

            Console.WriteLine();
        }

        // Sends the execute instruction to the powe
        // source_controller- The instance of the controller of the battler
        public void ExecutePower(Player source_controller)
        {
            this.power.Execute(source_controller, this);
        }

        // Attach one energy to the pokemon and recalculates the energy total
        // input- The energy to attach
        public void attachEnergy(energy input)
        {
            energies.Add(input);

            energyTotal[input.elem] += input.quan;

        }

        // Utility method to represent the battler
        public string BattleDescription()
        {
            string output = name + ": ";
            foreach (energy en in energies)
                output += en.ToProduction();
            output += Environment.NewLine;

            for (int i = 0; i < movements.Length; i++)
                if (isUsable(movements[i]) && !conditions.ContainsKey(i+1))
                {
                    movements[i].usable = true;
                    output += (i+1)+"- " + movements[i].ToString() + Environment.NewLine;
                }
                else
                    movements[i].usable = false;

            return (output);
        }

        // Generic ToString
        public override string ToString()
        {
            return (name);
        }

        // Checks energy conditions to determine if a movement is usable
        // mov- The movement to check usability
        private bool isUsable(movement mov)
        {
            int colorless = 0;
            for (int i = 6; i >= 1; i--)
            {
                if (mov.cost[i] > energyTotal[i]) // Check if the costs cannot be played
                    return (false);

                if (mov.cost[i] <= energyTotal[i]) // Check for excess of colored energy
                    colorless += energyTotal[i] - mov.cost[i]; // Add it to the colorless buffer
            }

            if (mov.cost[0] > colorless + energyTotal[0]) // Check if there is enough colorless energy
                return (false);

            return (true);
        }

        // Returns the supertype of the card, zero for pokemon cards
        public int getSuperType()
        {
            return (0);
        }

        // Returns the pokemon to its initial values
        public void clear()
        {
            this.damage = 0;
            this.status = 0;
            energies = new List<energy>();
            energyTotal = new int[7];
            conditions.Clear();
            this.res_elem = originalResElem;
            this.weak_elem = originalWeakElem;
            this.leekSlap = false;
        }

        // Prepares a battler to return to bench
        public void ToBench()
        {
            this.res_elem = originalResElem;
            this.weak_elem = originalWeakElem;
            this.status = 0;
            conditions.Clear();
        }

        // Creates a new instance of battler inheriting all the necessary data of the evolved battler
        public void evolve(battler pre)
        {
            this.damage = pre.damage;
            this.energies = pre.energies;
            this.energyTotal = pre.energyTotal;

            pre.clear();
            this.prevolutions.AddFirst(pre);
        }

        // Generic method to show the energies that a battler has
        public string showEnergy()
        {
            string output = "";

            for (int i = 0; i < energies.Count; i++)
                output += (i + 1) + "- " + energies[i].ToString() + Environment.NewLine;

            return output;
        }

        // Checks if the battler meets the conditions to retreat
        public bool canRetreat()
        {
            movement retreatProxy = new movement(new int[7]{retreat, 0, 0, 0, 0, 0, 0}, 0, "", null); // Create a proxy movement with the cost of retreating
            return isUsable(retreatProxy); // Return its usability
        }

        // Shows only the selected energy type
        // elem- The type of energy that must be shown
        public string ShowEnergyByType(int elem)
        {
            string buffer = this.name;

            foreach (energy en in energies)
                if (en.elem == elem)
                    buffer += " " + utilities.numToType(elem);

            return buffer;
        }

        // Indicates if the battler can execute a power
        public bool CanUsePowers()
        {
            return status != 1 && status != 2 && status != 3;
        }

        public battler DeepCopy()
        {
            movement[] neoMovements = new movement[movements.Length];

            for (int i = 0; i < movements.Length; i++)
                neoMovements[i] = movements[i].DeepCopy();

            Power neoPower = null;
            if (power != null)
                neoPower = power.DeepCopy();

            return new battler(this.type, this.element, this.HP, this.weak_elem, this.weak_mod, this.res_elem, this.res_mod, this.retreat, this.name, this.id, this.evolvesFrom, neoMovements, neoPower);
        }
    }
}
