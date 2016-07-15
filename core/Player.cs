﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shandakemon.core
{
    public class Player
    {
        public int id;
        public battler front;
        public List<battler> benched;
        public List<card> hand;
        public LinkedList<card> discarded, deck;
        public card[] prices;

        public Player(int id, LinkedList<card> deck, int nPrices)
        {
            this.id = id;
            this.deck = deck;
            this.shuffle();

            this.discarded = new LinkedList<card>();
            this.hand = new List<card>();
            this.prices = new card[nPrices];
            this.benched = new List<battler>();
        }

        public void shuffle()
        {
            this.deck = new LinkedList<card>(this.deck.OrderBy(x => CRandom.RandomInt()));
        }

        public bool draw(int times)
        {
            if (deck.Count < times)
                return false;
            for (int i = 0; i < times; i++)
            {
                hand.Add(deck.First());
                deck.RemoveFirst();
            }

            return true;
        }

        public void drawPrice(int index)
        {
            hand.Add(prices[index]);
            prices[index] = null;
        }

        public void putPrices()
        {
            for (int i = 0; i < prices.Length; i++)
            {
                this.prices[i] = deck.First();
                deck.RemoveFirst();
            }
        }

        public string writeHand()
        {
            string output = "";
            for (int i = 0; i < hand.Count; i++)
                output += (i + 1) + "- " + hand[i].ToString() + Environment.NewLine;
            return (output);
        }

        public string writeBattlers()
        {
            string output = "1- " + front.ToString() + Environment.NewLine;
            for (int i = 0; i < benched.Count; i++)
                output += (i + 2) + "- " + benched[i].ToString() + Environment.NewLine;
            return (output);
        }

        public string writeBenched()
        {
            string output = "";
            for (int i = 0; i < benched.Count; i++)
                output += (i + 1) + "- " + benched[i].ToString() + Environment.NewLine;
            return (output);
        }

        public bool[] listPrices()
        {
            bool[] output = new bool[prices.Length];
            for (int i = 0; i < prices.Length; i++)
                if (prices[i] != null)
                    output[i] = true;
            return output;
        }

        public void frontToDiscard()
        {
            while (front.energies.Count != 0)
            {
                energy en = front.energies[0];
                discarded.AddFirst(en);
                front.energies.Remove(en);
            }

            front.clear();
            discarded.AddFirst(front);
            front = null;

        }

        public void toFront(int index)
        {
            front = benched[index];
            benched.RemoveAt(index);
        }

        public void clearSickness()
        {
            front.sumSick = false;
            foreach (battler bt in benched)
                bt.sumSick = false;
        }

        public void discardEnergy(battler source, int energy_index)
        {
            card output = source.energies[energy_index];
            discarded.AddFirst(output);
            source.energies.RemoveAt(energy_index);
        }

        public void checkConditions()
        {
            foreach (var key in front.conditions.Keys.ToList())
            {
                if (front.conditions[key] == 1)
                    front.conditions.Remove(key);
                else
                    front.conditions[key]--;
            }
        }

        public bool checkInitialHand()
        {
            foreach (card inst in hand)
            {
                if (inst.getSuperType() == 0)
                {
                    battler temp = (battler)inst;
                    if (temp.type == 0) return true;
                }
            }

            return false;
        }
    }
}
