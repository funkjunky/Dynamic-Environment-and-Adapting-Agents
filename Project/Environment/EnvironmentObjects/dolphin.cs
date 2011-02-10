using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class dolphin : EnvironmentObject
    {
        // Eat fish
        double hungry = 0;
        BayesNet bnet;

        public dolphin(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "dolphin";
            this.category = "Animal";
            this.Speed = 2;
            this.iCanMove = true;
            this.DestroysCategory.Add("Water", true);
            this.cannotBeWith.Add("land", true);
            this.chanceToDuplicate = 0.02;
            this.maxSquare = 20;
        }

        public override bool isalive()
        {
            if (hungry >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string toString()
        {
            return ">";
        }

        public bool updateHungry()
        {
            // How would we even KNOW? At this point

            return true;
        }

        public override bool update()
        {
            this.updateHungry();
            return base.update();
        }

        public override bool duplicate()
        {
            return true;
        }
    }
}
