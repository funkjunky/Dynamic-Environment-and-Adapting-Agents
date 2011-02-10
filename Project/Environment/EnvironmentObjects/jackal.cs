using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    // ITS A JACKAL!
    class jackal : EnvironmentObject
    {
        // It gets hungry, so fed is silly.
        double hungry = 15;
        Dictionary<string, bool> eats = new Dictionary<string,bool>();

        BayesNet bnet;

        public jackal(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "jackal";
            this.category = "Animal";
            this.Speed = 1;
            this.iCanMove = true;
            this.cannotBeWith.Add("water", true);
            this.chanceToDuplicate = 0.2;
            this.cannotBeWith.Add("mountain", true);
            this.cannotBeWith.Add("tree", true);
            this.priority = 12;
            this.resouces.Add("meat", 16);
            this.maxSquare = 10;
            eats.Add("rabbit", true);
            eats.Add("fish", true);
        }

        public override bool isalive()
        {
            if ( hungry < 0 || age > 60)
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
            return "J";
        }

        public bool updateHungry()
        {
            if (hungry < 10)
            {
                HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
                foreach (EnvironmentObject E in O)
                {
                    if (hungry > 10)
                        return true;
                    if (eats.ContainsKey(E.name))
                        foreach (KeyValuePair<string, int> resource in E.resouces)
                            if (resource.Key.Equals("meat"))
                            {
                                //Console.WriteLine("I ate a(n) " + E.name + " at " + X + " & " + Y);
                                // I eat you
                                hungry = hungry + resource.Value;
                                E.iCanMove = false;
                                EnvironmentMap.remove(E, E.X, E.Y);
                            }
                }
            }

            --hungry;
            
            return true;
        }


        public override bool update()
        {
            this.updateHungry();
            return base.update();
        }

        public override bool duplicate()
        {
            if (age < 10 ||
                hungry < 10)
                return false;

            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            double chance;
            bool other = false;

            foreach (EnvironmentObject E in O)
                // if there are other jackals around.... try to mate with them
                if (E.name.Equals(this.name) && E.age > 10)
                    other = true;
                
            if (other)
            {
                chance = EnvironmentMap.Rand.NextDouble();
                if (chance < chanceToDuplicate)
                    EnvironmentMap.add(new jackal(bnet), X, Y);
            }

            return true;
        }
    }
}
