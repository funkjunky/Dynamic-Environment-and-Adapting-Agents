using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class mountain : EnvironmentObject
    {
        BayesNet bnet;

        public mountain(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "mountain";
            this.category = "Land";
            this.Speed = 0;
            this.iCanMove = false;
            this.DestroysCategory.Add("Land", true);
            this.chanceToDuplicate = 5;
            this.priority = 1;
            this.resouces.Add("stone", 25);
        }

        public override string toString()
        {
            return "^";
        }

        public override bool update()
        {
            this.resouces["stone"] += 2;
            return base.update();
        }
    }
}
