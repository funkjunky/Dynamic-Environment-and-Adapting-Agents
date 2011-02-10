using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class ground : EnvironmentObject
    {
        BayesNet bnet;

        public ground(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "ground";
            this.category = "Land";
            this.Speed = 0;
            this.iCanMove = false;
            this.DestroysCategory.Add("Land", true);
            this.DestroysCategory.Add("Fish", true);
            this.chanceToDuplicate = 0;
            this.priority = 1;
        }

        public override string toString()
        {
            return ".";
        }
    }
}
