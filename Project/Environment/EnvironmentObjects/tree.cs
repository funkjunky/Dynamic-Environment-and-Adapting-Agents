using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork.Classes;

namespace Project.Classes
{
    // Yes, I made you a tree.
    class tree : EnvironmentObject
    {
        double watered = 10;
        BayesNet bnet;

        public tree(BayesNet Bnet)
        {
            this.bnet = Bnet;
            this.name = "tree";
            this.category = "Plant";
            this.Speed = 0;
            this.iCanMove = false;
            this.cannotBeWith.Add("water", true);
            this.chanceToDuplicate = 0.00;
            this.priority = 10;
            this.resouces.Add("wood", 20);
            this.resouces.Add("syrup", 10);
            this.resouces.Add("fruit", 5);
            this.maxSquare = 10;
        }

        // Is there a destroys function? Because trees can just die.
        public override bool isalive()
        {
            
            if (watered <= 0 || age > 300)
            {
                EnvironmentMap.remove(this, X, Y);
                return false;
            }
            else
                return true;
        }

        public override string toString()
        {
            return "T";
        }


        public bool updateWatered()
        {
            //if (bnet["raining"].ObservedValue)
            if (bnet["precipitation"].ObservedValue == bnet["precipitation"].States.IndexOf("raining") ||
                    EnvironmentMap.near("water", X, Y, 1))
                {
                    this.resouces["wood"] += 1;
                    watered = watered + 2;
                    this.resouces["fruit"] += 2;
                }
                else
                    watered = watered - 1;

            return true;
        }

        public override bool update()
        {
            //if (bnet["lightning"].ObservedValue.HasValue)
            //{
                if (bnet["lightning"].ObservedValue == bnet["lightning"].States.IndexOf("true"))
                {
                    double chanceToLive = EnvironmentMap.Rand.NextDouble();
                    // hurricans can destroy trees right? Maybe... But Lightning destroys tree and hurricanes!
                    // like really... what is the chance of you being hit by lightning?
                    if (watered != 0 && chanceToLive < 0)
                        EnvironmentMap.remove(this, X, Y);
                }
            //}
            this.updateWatered();
            return base.update();
        }

        public override bool duplicate()
        {
            if (age > 30)
            {
                int dupl = age % 11;
                double chance = watered / 100;
                double Chances = EnvironmentMap.Rand.NextDouble();
                if (dupl == 0 &&
                    Chances < chance)
                {
                    //find out what direction it wants to spread in
                    HashSet<EnvironmentObject> O;

                    int spread = new int();
                    int offset = new int();
                    spread = EnvironmentMap.Rand.Next(1, 4);

                    switch (spread)
                    {
                        case 1:
                            offset = Y + 1;
                            if (offset >= EnvironmentMap.sizeY)
                                break;
                            O = EnvironmentMap.getAll(X, offset);

                            EnvironmentMap.add(new tree(bnet), X, offset);
                            break;
                        case 2:
                            offset = X + 1;
                            if (offset >= EnvironmentMap.sizeX)
                                break;
                            O = EnvironmentMap.getAll(offset, Y);

                            EnvironmentMap.add(new tree(bnet), offset, Y);
                            break;
                        case 3:
                            offset = Y - 1;
                            if (offset < 0)
                                break;
                            O = EnvironmentMap.getAll(X, offset);

                            EnvironmentMap.add(new tree(bnet), X, offset);
                            break;
                        case 4:
                            offset = X - 1;
                            if (offset < 0)
                                break;
                            O = EnvironmentMap.getAll(offset, Y);

                            EnvironmentMap.add(new tree(bnet), offset, Y);
                            break;
                    }
                }
            }

            return true;
        }
    }
}
