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
    class garden : EnvironmentObject
    {
        BayesNet bnet;
        int health = 0;
        House owner;

        public garden(BayesNet Bnet, House Owner)
        {
            this.bnet = Bnet;
            this.name = "garden";
            this.category = "Land";
            this.Speed = 0;
            this.iCanMove = false;
            this.cannotBeWith.Add("water", true);
            this.chanceToDuplicate = 0.00;
            this.priority = 16;
            this.resouces.Add("fruit", 0);
            this.maxSquare = 5;
            this.owner = Owner;
        }

        public override string toString()
        {
            return "%";
        }

        public bool updateWatered()
        {
            //if (bnet["raining"].ObservedValue.HasValue)
            if (bnet["precipitation"].ObservedValue == bnet["precipitation"].States.IndexOf("raining") ||
                    EnvironmentMap.near("water", X, Y, 1))
                    this.resouces["fruit"] += health;
                else
                {
                    this.resouces["fruit"] += health / 4;
                    --health;
                    if (health < 0)
                        health = 0;
                }
            return true;
        }

        public bool takeCare(Agent A)
        {
            // you can only take care of the garden if its part of your house
            if (A.Home == owner)
            {
                health += 5;
                return true;
            }
            return false;
        }

        public bool gather(Agent A)
        {
            // can the agent gather from this garden?
            if (A.Home == owner)
                return true;
            return false;
        }

        public override bool update()
        {
            this.updateWatered();
            return base.update();
        }

        public override bool duplicate()
        {
            return false;
        }
    }
}
