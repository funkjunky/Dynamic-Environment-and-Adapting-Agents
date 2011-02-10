using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class shark : EnvironmentObject
    {
        // Eats fish and dolphin
        double hungry = 40;
        BayesNet bnet;
        Dictionary<string, bool> eats = new Dictionary<string, bool>();

        public shark(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "shark";
            this.category = "Fish";
            this.Speed = 4;
            this.iCanMove = true;
            this.cannotBeWith.Add("ground", true);
            this.cannotBeWith.Add("mountain", true);
            this.chanceToDuplicate = 0.2;
            this.priority = 13;
            this.resouces.Add("meat", 30);
            this.maxSquare = 10;
            eats.Add("fish", true);
        }

        public override bool isalive()
        {
            if (hungry >= 1 || age > 60)
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
            return "S";
        }

        public bool updateHungry()
        {
            if (hungry < 30)
            {
                HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
                foreach (EnvironmentObject E in O)
                {
                    if (hungry > 30)
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

            hungry = hungry - 2;

            return true;
        }

        public override bool update()
        {
            this.updateHungry();
            return base.update();
        }

        public override bool duplicate()
        {
            if (age < 20 ||
                 hungry < 20)
                return false;

            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            double chance;
            bool other = false;

            foreach (EnvironmentObject E in O)
            {
                // if there are other shark around.... try to mate with them
                if (E.name.Equals(this.name) && E.age > 19)
                    other = true;
            }
            if (other)
            {
                chance = EnvironmentMap.Rand.NextDouble();
                if (chance < chanceToDuplicate)
                    EnvironmentMap.add(new shark(bnet), X, Y);
            }
            return true;
        }
    }
}
