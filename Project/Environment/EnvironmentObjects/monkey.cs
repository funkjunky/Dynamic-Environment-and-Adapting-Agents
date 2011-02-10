using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class monkey : EnvironmentObject
    {
        // It gets hungry, so fed is silly.
        double hungry = 0;
        BayesNet bnet;

        public monkey(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "monkey";
            this.category = "Animal";
            this.Speed = 2;
            this.iCanMove = true;
            this.DestroysCategory.Add("Land", true);
            this.cannotBeWith.Add("water", true);
            this.chanceToDuplicate = 0.02;
            this.maxSquare = 25;
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
            return "m";
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
