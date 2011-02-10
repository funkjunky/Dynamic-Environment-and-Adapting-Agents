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
    class House : EnvironmentObject
    {
        BayesNet bnet;
        private int gardens = 0;
        private int woodmile = 0;
        public int MaxCapacity = 10;
        public List<Agent> currentOccupants = new List<Agent>();
        public int MaxStorage = 200;
        public const double optimal = 0.5;
        public int currentStorage = 0;
        public Dictionary<string, int> Inventory = new Dictionary<string, int>();
        public int currentLevel = 0;
        public Dictionary<int, Dictionary<string, int>> Upgrades = new Dictionary<int, Dictionary<string, int>>();

        public House(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "House";
            this.category = "Land";
            this.Speed = 0;
            this.iCanMove = false;
            this.chanceToDuplicate = 0;
            this.priority = 20;
            this.cannotBeWith.Add("water", true);
            // setting up the requirements for upgrading
            int j = 0;
            Dictionary<string, int> requirements = new Dictionary<string,int>();
            while(j < 6)
            {
                switch (j)
                {
                    case 0:
                        requirements.Add("wood", 50);
                        requirements.Add("stone", 0);
                        requirements.Add("NewStorage", 300);
                        requirements.Add("NewCapacity", 20);
                        Upgrades.Add(j, new Dictionary<string,int>(requirements));
                        break;
                    case 1:
                        requirements["wood"] =  50;
                        requirements["stone"] = 25;
                        requirements["NewStorage"] = 500;
                        requirements["NewCapacity"] = 30;
                        Upgrades.Add(j, new Dictionary<string, int>(requirements));
                        break;
                    case 2:
                        requirements["wood"] = 75;
                        requirements["stone"] = 50;
                        requirements["NewStorage"] = 700;
                        requirements["NewCapacity"] = 50;
                        Upgrades.Add(j, new Dictionary<string, int>(requirements));
                        break;
                    case 3:
                        requirements["wood"] = 100;
                        requirements["stone"] = 150;
                        requirements["NewStorage"] = 1000;
                        requirements["NewCapacity"] = 75;
                        Upgrades.Add(j, new Dictionary<string, int>(requirements));
                        break;
                    case 4:
                        requirements["wood"] = 150;
                        requirements["stone"] = 200;
                        requirements["NewStorage"] = 2000;
                        requirements["NewCapacity"] = 100;
                        Upgrades.Add(j, new Dictionary<string, int>(requirements));
                        break;
                    case 5:
                        requirements["wood"] = 300;
                        requirements["stone"] = 300;
                        requirements["NewStorage"] = 2500;
                        requirements["NewCapacity"] = 150;
                        Upgrades.Add(j, new Dictionary<string, int>(requirements));
                        break;
                }
                ++j;
            }
        }

        public override string toString()
        {
            return "#";
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
            foreach (KeyValuePair<string, int> resource in a.Inventory)
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
            while (a.hungry < 30
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
            while (a.hungry < 30 
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

            //We might need a garden... lets find out
            if (currentLevel > 0 && this.gardens < Math.Pow(currentLevel, 1.5))
            {
                // we are high enough level to build a garden
                if (Inventory.ContainsKey("fruit"))
                {
                    // we have enough fruit to create a garden
                    int NewX = X;
                    int NewY = Y;
                    int randomD = EnvironmentMap.Rand.Next(1, 4);
                    switch (randomD)
                    {
                        case 1:
                            ++NewX;
                            break;
                        case 2:
                            --NewX;
                            break;
                        case 3:
                            ++NewY;
                            break;
                        case 4:
                            --NewY;
                            break;
                    }

                    EnvironmentMap.add(new garden(bnet, this), NewX, NewY);
                    ++this.gardens;
                }
            }
            // how about a wood mill?
            if (currentLevel > 1 && this.woodmile < Math.Pow(currentLevel, 1))
            {
                // we are high enough level to build a garden
                if (Inventory.ContainsKey("wood"))
                {
                    // we have enough fruit to create a garden
                    int NewX = X;
                    int NewY = Y;
                    int randomD = EnvironmentMap.Rand.Next(1, 4);
                    switch (randomD)
                    {
                        case 1:
                            ++NewX;
                            ++NewY;
                            break;
                        case 2:
                            --NewX;
                            ++NewY;
                            break;
                        case 3:
                            ++NewX;
                            --NewY;
                            break;
                        case 4:
                            --NewX;
                            --NewY;
                            break;
                    }

                    EnvironmentMap.add(new woodmile(bnet, this), NewX, NewY);
                    ++this.woodmile;
                }
            }


            // Now we are well fead, lets go back to the hunt!
            if (currentOccupants.Count > (MaxCapacity / 2)
                && Inventory.ContainsKey("meat") && Inventory.ContainsKey("fruit") 
                && (Inventory["meat"] + Inventory["fruit"]) > (optimal * MaxStorage))
            {
                if (currentLevel < 6)
                {
                    Dictionary<string, int> requirements = Upgrades[currentLevel];
                    if (Inventory.ContainsKey("wood") && Inventory["wood"] >= requirements["wood"])
                    {
                        if (Inventory.ContainsKey("stone") && Inventory["stone"] >= requirements["stone"])
                        {
                            // let's upgrade the house now.
                            Inventory["wood"] -= requirements["wood"];
                            currentStorage -= requirements["wood"];
                            Inventory["stone"] -= requirements["stone"];
                            currentStorage -= requirements["stone"];
                            MaxCapacity = requirements["NewCapacity"];
                            MaxStorage = requirements["NewStorage"];
                            ++currentLevel;
                            a.goal = "hunt";
                        }
                        else
                            a.goal = "collectStone";
                    }
                    else
                        a.goal = "collectWood";
                }
                else
                {
                    a.goal = "hunt";
                    // we are at max lvl inside the house
                }

            }
            else
            {
                a.goal = "hunt";
            }

            return true;
        }
    }
}

