using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class rabbit : EnvironmentObject
    {
        BayesNet bnet;

        public rabbit(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "rabbit";
            this.category = "Animal";
            this.Speed = 2;
            this.iCanMove = true;
            this.cannotBeWith.Add("water", true);
            this.chanceToDuplicate = 0.4;
            this.priority = 11;
            this.resouces.Add("meat", 12);
            this.maxSquare = 25;
        }

        public override bool isalive()
        {
            // rabits dont live long
            if (age > 30)
            {
                this.iCanMove = false;
                EnvironmentMap.remove(this, X, Y);
                return false;
            }
            return true;
        }

        public override string toString()
        {
            return "R";
        }

        public override bool update()
        {
            return base.update();
        }
        
        public override bool duplicate()
        {
            if (age < 5 || age > 20)
                return false;

            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            double chance;
            bool other = false, tree = false;

            foreach (EnvironmentObject E in O)
            {
                // if there are other rabbits around.... try to mate with them
                if (E.name.Equals(this.name) && E.age > 4)
                    other = true;
                // they need trees to make a baby
                if (E.name.Equals("tree"))
                    tree = true;
            }
            if(other && tree)
            {
                chance = EnvironmentMap.Rand.NextDouble();
                if (chance < chanceToDuplicate)
                    EnvironmentMap.add(new rabbit(bnet), X, Y);
            }
            return true;
        }
    }
}
