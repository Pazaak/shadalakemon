using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    public class battler : card
    {
        public int type, element, HP, weak_elem, weak_mod, res_elem, res_mod, retreat, damage, status, id, evolvesFrom;
        public string name;
        public movement[] movements;
        public List<energy> energies;
        private int[] energyTotal;
        public bool sumSick;
        public LinkedList<battler> prevolutions;

        // types
        // 0: Basic
        // 1: Evolution
        public battler(int type, int element, int HP, int weak_elem, int weak_mod, int res_elem, int res_mod, int retreat, string name, int id, int evolvesFrom, movement[] movements)
        {
            this.type = type;
            this.element = element;
            this.HP = HP;
            this.weak_elem = weak_elem;
            this.weak_mod = weak_mod;
            this.res_elem = res_elem;
            this.res_mod = res_mod;
            this.retreat = retreat;
            this.name = name;
            this.id = id;
            this.evolvesFrom = evolvesFrom;
            this.damage = 0;
            this.status = 0;
            this.movements = movements;
            this.sumSick = true;
            energies = new List<energy>();
            energyTotal = new int[7];
            this.prevolutions = new LinkedList<battler>();
        }

        public void execute(int index, battler target)
        {
            Console.WriteLine();
            movement selected = movements[index];

            if (selected.usable)
                Console.WriteLine("Did " +selected.execute(target));
            else
                Console.WriteLine("Not enough energy to use that");

            Console.WriteLine();
        }

        public void attachEnergy(energy input)
        {
            energies.Add(input);

            energyTotal[input.elem] += input.quan;

        }

        public string BattleDescription()
        {
            string output = name + ": ";
            foreach (energy en in energies)
                output += en.ToProduction();
            output += Environment.NewLine;
            for (int i = 0; i < movements.Length; i++)
            {
                movement mov = movements[i];

                if (isUsable(mov))
                {
                    mov.usable = true;
                    output += (i + 1) + "- " + mov.ToString() + Environment.NewLine;
                }
                else
                    mov.usable = false;
            }
            return (output);
        }


        public override string ToString()
        {
            return (name);
        }

        private bool isUsable(movement mov)
        {
            int colorless = 0;
            for (int i = 6; i >= 1; i--)
            {
                if (mov.cost[i] > energyTotal[i])
                    return (false);

                if (mov.cost[i] == 0)
                    colorless += energyTotal[i];
            }

            if (mov.cost[0] > colorless + energyTotal[0])
                return (false);

            return (true);
        }

        public int getSuperType()
        {
            return (0);
        }

        public void clear()
        {
            this.damage = 0;
            this.status = 0;
            energies = new List<energy>();
            energyTotal = new int[7];
        }

        public void evolve(battler pre)
        {
            this.damage = pre.damage;
            this.energies = pre.energies;
            this.energyTotal = pre.energyTotal;

            pre.clear();
            this.prevolutions.AddFirst(pre);
        }

    }
}
