using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class dam : EnvironmentObject
    {
        BayesNet bnet;

        public dam(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "dam";
            this.category = "Land";
            this.Speed = 0;
            this.iCanMove = false;
            this.chanceToDuplicate = 0;
            this.priority = 16;
        }

        public override string toString()
        {
            return "|";
        }

        public override bool isalive()
        {

            if (age > 200)
            {
                EnvironmentMap.remove(this, X, Y);
                return false;
            }
            else
                return true;
        }
    }
}
