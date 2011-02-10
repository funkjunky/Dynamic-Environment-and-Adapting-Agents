using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Environment.Classes
{
    class grass : EnvironmentObject
    {
        double watered = 10;
        int age = 1;
        // Should it have a value of how much grass is in a square?

        BayesNet bnet;

        public grass(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "grass";
            this.category = "Plant";
            this.Speed = 0;
            this.iCanMove = false;
            this.cannotBeWith.Add("water", true);
            this.chanceToDuplicate = 0.1;
            this.priority = 15;
        }

        // Is there a destroys function? Because trees can just die.
        public override bool isalive()
        {
            if (watered <= 0 || age > 10)
            {
                EnvironmentMap.remove(this, X, Y);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override string toString()
        {
            return ",";
        }

        public bool updateAge()
        {
            ++age;
            return true;
        }

        public bool updateWatered()
        {
            if (bnet["raining"].ObservedValue.HasValue)
                if (bnet["raining"].ObservedValue.Value ||
                    EnvironmentMap.near("water", X, Y, 1))
                    watered = watered + 2;
                else
                    watered = watered - 1;
            
            return true;
        }

        public override bool update()
        {
            this.updateWatered();
            this.updateAge();
            return base.update();
        }

        public override bool duplicate()
        {
            return false;   //This is just a place holder!! 
            //I have no idea if this is correct. 
            //Just trying to get the project to build
        }
    }
}
