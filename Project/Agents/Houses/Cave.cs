using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;
using Project.Classes;

namespace Project
{
    class Cave: EnvironmentObject
    {
        BayesNet bnet;
        public const int MaxCapacity = 20;
        public List<Agent> currentOccupants = new List<Agent>();
        public const int MaxStorage = 300;
        public const double optimal = 0.6;
        public int currentStorage = 0;
        public Dictionary<string, int> Inventory = new Dictionary<string,int>();

        public Cave(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "Cave";
            this.category = "Land";
            this.Speed = 0;
            this.iCanMove = false;
            this.chanceToDuplicate = 0;
            this.priority = 0;
        }

        public override string toString()
        {
            return "^";
        }

        public bool moveIn(Agent a)
        {
            if (currentOccupants.Count >= MaxCapacity)
                return false; // you can not move in here... its full

            // We should probably add stuff like if that person is liked or family then thay can move in
            // else they can not
            currentOccupants.Add(a);
            //Console.WriteLine("We now have " + currentOccupants.Count + " people living here.");
            return true;
        }

        public bool moveOut(Agent a)
        {
            if (currentOccupants.Contains(a))
            {
                currentOccupants.Remove(a);
                //Console.WriteLine("We now have " + currentOccupants.Count + " people living here.");
                a.Home = null;
            }
            else
                return false; // that person does not live here dummy

            return true;
        }

        public bool relax(Agent a)
        {

            if (!this.currentOccupants.Contains(a))
                return false; // you can not chill here
            // now while the agent is chilling they put all there inventory in the house
            int spaceLeft = MaxStorage - currentStorage;
            foreach(KeyValuePair<string, int> resource in a.Inventory)
            {
                int Gathered = Math.Min(spaceLeft, resource.Value);
                if (Inventory.ContainsKey(resource.Key))
                    Inventory[resource.Key] += Gathered;
                else
                    Inventory.Add(resource.Key, Gathered);
                currentStorage += Gathered;
            }
            a.Inventory.Clear();
            a.AmountInventory = 0;
            // Ok now that everything is in the house... lets chill
            // First we eat fruits!
            while (a.hungry < 25
                   && Inventory.ContainsKey("fruit"))
            {
                // while we are hungry.. we eat!
                if (Inventory["fruit"] > 5) // if we have enough food
                {
                    a.hungry += 5;
                    Inventory["fruit"] -= 5;
                    currentStorage -= 5;
                }
                else
                    break; //the house needs more food :(
            }
            // if we are still hungry... try some meat
            while (a.hungry < 25
                && Inventory.ContainsKey("meat"))
            {
                // while we are hungry.. we eat!
                if (Inventory["meat"] > 5) // if we have enough food
                {
                    a.hungry += 10;
                    Inventory["meat"] -= 5;
                    currentStorage -= 5;
                }
                else
                    break; //the house needs more food :(
            }

            // Now we are well fead, lets go back to the hunt!
            if (currentOccupants.Count > (MaxCapacity / 2)
                && Inventory.ContainsKey("meat") && Inventory.ContainsKey("fruit")
                && (Inventory["meat"] + Inventory["fruit"]) > (optimal * MaxStorage))
            {
                if (Inventory.ContainsKey("wood") && Inventory["wood"] > 50)
                {
                    if (a.Inventory.ContainsKey("wood"))
                        a.Inventory["wood"] += 50;
                    else
                        a.Inventory.Add("wood", 50);

                    Inventory["wood"] -= 50;
                    currentStorage -= 50;
                    a.goal = "buildHouse";
                    this.moveOut(a);
                }
                else
                    a.goal = "collectWood";

            }
            else
            {
                a.goal = "hunt";
            }

            return true;
        }
    }
}

