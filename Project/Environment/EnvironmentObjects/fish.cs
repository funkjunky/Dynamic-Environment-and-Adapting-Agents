using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class fish : EnvironmentObject
    {
        BayesNet bnet;

        public fish(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "fish";
            this.category = "Fish";
            this.Speed = 2;
            this.iCanMove = true;
            this.cannotBeWith.Add("ground", true);
            this.cannotBeWith.Add("mountain", true);
            this.chanceToDuplicate = 0.3;
            this.priority = 11;
            this.resouces.Add("meat", 5);
            this.maxSquare = 25;
        }

        public override bool isalive()
        {
            if (age > 30)
            {
                this.iCanMove = false;
                EnvironmentMap.remove(this, X, Y);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string toString()
        {
            return "<";
        }

        public override bool update()
        {
            return base.update();
        }

        public override bool duplicate()
        {
            if (age < 10 ||
                 age > 25)
                return false;

            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            double chance;
            bool other = false;

            foreach (EnvironmentObject E in O)
            {
                // if there are other fish around.... try to mate with them
                if (E.name.Equals(this.name) && E.age > 9)
                    other = true;
            }
            if (other)
            {
                chance = EnvironmentMap.Rand.NextDouble();
                if (chance < chanceToDuplicate)
                    EnvironmentMap.add(new fish(bnet), X, Y);
            }
            return true;
        }
    }
}
