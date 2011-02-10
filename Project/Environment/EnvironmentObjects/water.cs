using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class water : EnvironmentObject
    {
        int dry = 30;
        BayesNet bnet;


        public water(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "water";
            this.category = "Land";
            this.Speed = 0;
            this.iCanMove = false;
            this.priority = -1;
            this.DestroysCategory.Add("Land", true);
            this.DestroysCategory.Add("Plant", true);
            this.DestroysCategory.Add("Animal", true);
            this.cannotBeWith.Add("mountain", true);
            this.cannotBeWith.Add("dam", true);
            this.chanceToDuplicate = 0.05;
        }

        public bool updateDry()
        {
            //if(bnet["raining"].ObservedValue.HasValue)
            if (bnet["precipitation"].ObservedValue == bnet["precipitation"].States.IndexOf("raining") ||
                    EnvironmentMap.near("water", X, Y, 3))
                {
                    if (chanceToDuplicate == 0)
                        chanceToDuplicate = 0.05;
                    chanceToDuplicate *= 1.5;
                    if (chanceToDuplicate > 1)
                        chanceToDuplicate = 1;
                    if (chanceToDuplicate > 0.5)
                        dry = dry + 3;
                    else
                        ++dry;
                }
                else
                {
                    //if (bnet["hot"].ObservedValue.HasValue)
                        if (bnet["temperature"].ObservedValue == bnet["temperature"].States.IndexOf("hot"))
                        {
                            double chanceToLive = EnvironmentMap.Rand.NextDouble();
                            // its really hot... water gose bye bye
                            if (chanceToLive > dry / 60)
                                dry = dry - 2;
                        }

                    chanceToDuplicate *= 0.6;
                    if (chanceToDuplicate < 0.005)
                        chanceToDuplicate = 0;
                    if (chanceToDuplicate == 0)
                        dry = dry - 2;
                    else
                        --dry;
                }

            return true;
        }

        public override bool duplicate()
        {
            double chance = new double();
            chance = EnvironmentMap.Rand.NextDouble();
            if (chance < chanceToDuplicate)
            {
                //find out what direction it wants to spread in
                HashSet<EnvironmentObject> O;

                int wander = new int();
                int offset = new int();
                wander = EnvironmentMap.Rand.Next(1, 4);
                bool candup = true;

                switch (wander)
                {
                    case 1:
                        offset = Y + 1;
                        if (offset >= EnvironmentMap.sizeY)
                            break;
                        O = EnvironmentMap.getAll(X, offset);
                        foreach (EnvironmentObject E in O)
                            if (E.name.Equals(this.name))
                                candup = false;

                        if (candup)
                            EnvironmentMap.add(new water(bnet), X, offset);
                        break;
                    case 2:
                        offset = X + 1;
                        if (offset >= EnvironmentMap.sizeX)
                            break;
                        O = EnvironmentMap.getAll(offset, Y);
                        foreach (EnvironmentObject E in O)
                            if (E.name.Equals(this.name))
                                candup = false;

                        if (candup)
                            EnvironmentMap.add(new water(bnet), offset, Y);
                        break;
                    case 3:
                        offset = Y - 1;
                        if (offset < 0)
                            break;
                        O = EnvironmentMap.getAll(X, offset);
                        foreach (EnvironmentObject E in O)
                            if (E.name.Equals(this.name))
                                candup = false;

                        if (candup)
                            EnvironmentMap.add(new water(bnet), X, offset);
                        break;
                    case 4:
                        offset = X - 1;
                        if (offset < 0)
                            break;
                        O = EnvironmentMap.getAll(offset, Y);
                        foreach (EnvironmentObject E in O)
                            if (E.name.Equals(this.name))
                                candup = false;

                        if(candup)
                            EnvironmentMap.add(new water(bnet), offset, Y);
                        break;
                }
            }
            return true;
        }

        public override bool isalive()
        {
            if (dry < 0)
            {
                EnvironmentMap.add(new ground(bnet), X, Y);
                return false;
            }
            else
                return true;
        }

        public override string toString()
        {
            return "~";
        }

        public override bool update()
        {
            this.updateDry();
            return base.update();
        }
    }
}
