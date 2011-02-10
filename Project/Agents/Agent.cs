using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using BayesNetwork.Classes;

namespace Project.Classes
{
    class Agent : EnvironmentObject
    {
        public double hungry = 30;
        public const int MaxStorage = 50;
        private Dictionary<string, bool> hunts = new Dictionary<string,bool>();
        private Dictionary<string, bool> gathers = new Dictionary<string, bool>();
        public Dictionary<string, int> Inventory = new Dictionary<string, int>();
        public int AmountInventory = 0;
        public EnvironmentObject Home;
        public string goal;
        private Stack<int[]> path = null;
        private Astar astar = new Astar();
        // social related variables
        public List<Agent> Family = new List<Agent>();
        public static ArrayList[,] myMap;

        BayesNet bnet;

        public Agent(BayesNet Bnet, EnvironmentObject home)
        {
            this.bnet = Bnet;
            this.name = "Agent";
            this.category = "Animal";
            this.Speed = 1;
            this.iCanMove = true;
            this.cannotBeWith.Add("water", true);
            this.chanceToDuplicate = 0.8;
            this.priority = 15;
            this.resouces.Add("meat", 25);
            this.maxSquare = 500;
            this.goal = "hunt";
            if (home.name.Equals("Cave"))
            {
                Cave cave = (Cave)home;
                if (cave.moveIn(this))
                    this.Home = cave;
                else
                    this.goal = "findNewHouse";
            }
            else if (home.name.Equals("House"))
            {
                House house = (House)home;
                if (house.moveIn(this))
                    this.Home = house;
                else
                    this.goal = "findNewHouse";
            }
            hunts.Add("rabbit", true);
            // can gather fruits from trees
            hunts.Add("tree", true);
            hunts.Add("garden", true);
            gathers.Add("tree", true);
            gathers.Add("woodmill", true);
            gathers.Add("mountain", true);

            this.Family.Add(this);

            myMap = new ArrayList[EnvironmentMap.sizeX, EnvironmentMap.sizeY];
            // Initialize all ArrayLists to default size (empty)
            for (int row = 0; row < EnvironmentMap.sizeX; ++row)
            {
                for (int column = 0; column < EnvironmentMap.sizeY; ++column)
                {
                    myMap[row, column] = new ArrayList();

                }
            }
        }
        public bool teach(ArrayList[,] map)
        {
            myMap = map;
            return true;
        }
        public override bool isalive()
        {
            if ( hungry < 0 || age > 60)
            {
                this.iCanMove = false;
                this.Die();
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
            return "A";
        }

        private bool amIAtHome()
        {
            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            foreach (EnvironmentObject E in O)
                if (E == Home)
                    return true;
            return false;
        }

        public bool collectWood()
        {
            if (AmountInventory < MaxStorage)
            {
                HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
                foreach (EnvironmentObject E in O)
                {
                    int RemaningInvetory = MaxStorage - AmountInventory;
                    if (RemaningInvetory <= 0) // hopefully it will never be less then 0
                        return true;
                    try
                    {
                        if (gathers.ContainsKey(E.name))
                            foreach (KeyValuePair<string, int> resource in E.resouces)
                                if (resource.Key.Equals("wood"))
                                {
                                    if (E.name.Equals("woodmill"))
                                    {
                                        woodmile woodmill = (woodmile)E;
                                        if (!woodmill.gather(this))
                                            return false;
                                    }

                                    //Console.WriteLine("I hunted a(n) " + E.name + " at " + X + " & " + Y);
                                    // I hunted you
                                    int Gathered = Math.Min(RemaningInvetory, resource.Value);
                                    if (!Inventory.ContainsKey("wood"))
                                        Inventory.Add("wood", Gathered);
                                    else
                                        Inventory["wood"] += Gathered;

                                    AmountInventory += Gathered;
                                    if (E.name.Equals("woodmill"))
                                        // we took wood from a woodmill
                                        E.resouces["wood"] -= Gathered;
                                    else
                                    {
                                        // we cut down a tree
                                        E.iCanMove = false;
                                        EnvironmentMap.remove(E, E.X, E.Y);
                                    }
                                }
                    }
                    catch(Exception)
                    { }
                }
            }
            return true;
        }

        public bool collectStone()
        {
            if (AmountInventory < MaxStorage)
            {
                HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
                foreach (EnvironmentObject E in O)
                {
                    int RemaningInvetory = MaxStorage - AmountInventory;
                    if (RemaningInvetory <= 0) // hopefully it will never be less then 0
                        return true;
                    if (gathers.ContainsKey(E.name))
                        foreach (KeyValuePair<string, int> resource in E.resouces)
                            if (resource.Key.Equals("stone"))
                            {
                                //Console.WriteLine("I hunted a(n) " + E.name + " at " + X + " & " + Y);
                                // I hunted you
                                int Gathered = Math.Min(RemaningInvetory, resource.Value);
                                if (!Inventory.ContainsKey("stone"))
                                    Inventory.Add("stone", Gathered);
                                else
                                    Inventory["stone"] += Gathered;
                                AmountInventory += Gathered;

                                E.resouces["stone"] -= Gathered;
                                return true;
                            }
                }
            }
            return true;
        }

        public bool huntFood()
        {
            if (AmountInventory < MaxStorage)
            {
                HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
                foreach (EnvironmentObject E in O)
                {
                    int RemaningInventory = MaxStorage - AmountInventory;
                    if (RemaningInventory <= 0) // hopefully it will never be less then 0
                        return true;
                    try
                    {
                        if (hunts.ContainsKey(E.name))
                            foreach (KeyValuePair<string, int> resource in E.resouces)
                                if (resource.Key.Equals("fruit"))
                                {
                                    if (E.name.Equals("garden"))
                                    {
                                        garden Garden = (garden)E;
                                        if (!Garden.gather(this))
                                            return false;
                                    }
                                        
                                    //Console.WriteLine("I harvested a(n) " + E.name + " at " + X + " & " + Y);
                                    int Gathered = Math.Min(RemaningInventory, resource.Value);
                                    if (!Inventory.ContainsKey("fruit"))
                                        Inventory.Add("fruit", Gathered);
                                    else
                                        Inventory["fruit"] += Gathered;
                                    AmountInventory += Gathered;

                                    E.resouces["fruit"] -= Gathered;
                                }
                                else if (resource.Key.Equals("meat"))
                                {
                                    //Console.WriteLine("I hunted a(n) " + E.name + " at " + X + " & " + Y);
                                    // I hunted you
                                    int Gathered = Math.Min(RemaningInventory, resource.Value);
                                    if (!Inventory.ContainsKey("meat"))
                                        Inventory.Add("meat", Gathered);
                                    else
                                        Inventory["meat"] += Gathered;
                                    AmountInventory += Gathered;

                                    E.iCanMove = false;
                                    EnvironmentMap.remove(E, E.X, E.Y);
                                }
                    }
                    catch (Exception)
                    { }
                }
            }
            
            return true;
        }


        public override bool update()
        {
            this.think();
            return base.update();
        }

        private void think()
        {
            //Console.WriteLine("My current goal is: " + this.goal);
            --hungry;
            this.remember();
            this.takeCare();

            if (this.goal.Equals("findNewHouse"))
            {
                HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
                // is there a house here?
                foreach (EnvironmentObject E in O)
                    if (E.name.Equals("Cave"))
                    {
                        // we found a cave... try to move in
                        Cave cave = (Cave)E;
                        if (cave.moveIn(this))
                        {
                            //Console.WriteLine("Found a new home!");
                            this.Home = cave;
                            this.goal = "hunt";
                        }
                    }
                    else if (E.name.Equals("House"))
                    {
                        // we found a house... try to move in
                        House house = (House)E;
                        if (house.moveIn(this))
                        {
                            //Console.WriteLine("Found a new home!");
                            this.Home = house;
                            this.goal = "hunt";
                        }
                    }
                
                return;
            }

            if (this.goal.Equals("buildHouse"))
            {
                this.buildHouse();
                return;
            }

            if (this.goal.Equals("collectWood"))
                this.collectWood();

            if (this.goal.Equals("collectStone"))
                this.collectStone();

            if (this.goal.Equals("hunt"))
                this.huntFood();
            
            if(this.goal.Equals("stayHome"))
            {
                if (Home.name.Equals("Cave"))
                {
                    Cave home = (Cave)Home;
                    home.relax(this);
                }
                else if (Home.name.Equals("House"))
                {
                    House house = (House)Home;
                    house.relax(this);
                }
            }

            if (!this.goal.Equals("goHome") && Home != null)
            {
                if ((MaxStorage - AmountInventory) < 10)
                {
                    //Console.WriteLine("My inventory is at " + AmountInventory + ". I am going home.");
                    path = astar.findPath(this, Home.X, Home.Y);
                    this.goal = "goHome"; // You are running out of space... go back home
                }
                else if (hungry <= 15)
                {
                    //Console.WriteLine("My hunger is at " + hungry + ". I am going home.");
                    path = astar.findPath(this, Home.X, Home.Y);
                    this.goal = "goHome"; // you are hungry and need to be home to eat
                }
            }
            else
            {
                this.buildDam(); // If we have stone we can build a dam to protect the house and the people
                this.huntFood(); // while heading home... if we see somthing to kill lets kill it
            }
        }

        public bool remember()
        {
            // the agents uses this to remember where certain things are, like mountains and farms
            myMap[X, Y] = EnvironmentMap.envMap[X, Y];
            return true;
        }

        private bool takeCare()
        {
            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            foreach (EnvironmentObject E in O)
            {
                if (E.name.Equals("garden"))
                {
                    garden Garden = (garden)E;
                    if(Garden.gather(this))
                        Garden.takeCare(this);
                }
                else if (E.name.Equals("woodmill"))
                {
                    woodmile Woodmill = (woodmile)E;
                    if (Woodmill.gather(this))
                        Woodmill.takeCare(this);
                }
            }
            return true;
        }

        public override bool move()
        {
            if (this.goal.Equals("goHome") && Home != null)
            {
                if (path.Count <= 0)
                {
                    if (!amIAtHome())
                        path = astar.findPath(this, Home.X, Home.Y);
                    else
                        this.goal = "stayHome";
                    return true;
                } 
                int[] next = path.Pop();

                EnvironmentMap.move(this, next[0], next[1]);
                return true;
            }
            else if (this.goal.Equals("collectStone"))
            {
                for (int column = 0; column < EnvironmentMap.sizeY; ++column)
                    for (int row = 0; row < EnvironmentMap.sizeX; ++row)
                        foreach (EnvironmentObject E in myMap[row, column])
                            if (E.name.Equals("mountain"))
                            {
                                try
                                {
                                    path = astar.findPath(this, E.X, E.Y);
                                    int[] next = path.Pop();

                                    EnvironmentMap.move(this, next[0], next[1]);
                                }
                                catch (Exception)
                                {
                                }
                                return true;
                            }

                // could not find a mountain that we knew of which is kinda odd.
                return base.move();
            }
            else
                return base.move();
        }

        private bool buildHouse()
        {
            bool goodPlace = false;
            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            foreach (EnvironmentObject E in O)
                if (E.name.Equals("tree"))
                    goodPlace = true;
            if (this.hungry <= 10)
                // i am to hungry to care
                goodPlace = true;
            if(goodPlace)
            {
                House house = new House(this.bnet);
                EnvironmentMap.add(house, X, Y);
                house.moveIn(this);
                this.Home = house;
                this.goal = "hunt";
            }
            else
                return false;
            return true;
        }

        private bool buildDam()
        {
            if (Inventory.ContainsKey("stone")
                && Inventory["stone"] > 5)
            {
                HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
                bool good = true;
                foreach (EnvironmentObject E in O)
                    if (E.name.Equals("dam"))
                        good = false;

                if (good
                    && EnvironmentMap.near("water", X, Y, 1))
                {
                    Inventory["stone"] -= 5;
                    this.AmountInventory -= 5;
                    EnvironmentMap.add(new dam(bnet), X, Y);
                }
            }
            return true;
        }

        public bool addFamily(Agent a)
        {
            if (Family.Contains(a) || a.Family.Contains(this))
                return false;

            Family.Add(a);
            a.Family.Add(this);
            return true;
        }

        public bool addFamily(List<Agent> List)
        {
            List<Agent> list = new List<Agent>(List);
            foreach (Agent a in list)
            {
                if (!this.Family.Contains(a))
                {
                    this.Family.Add(a);
                    a.Family.Add(a);
                }
            }
            return true;
        }

        public override bool duplicate()
        {
            if (age < 18 || age > 50 || 
                hungry < 5)
                return false;

            HashSet<EnvironmentObject> O = EnvironmentMap.getAll(X, Y);
            double chance;
            bool other = false;

            foreach (EnvironmentObject E in O)
                // if there are other agents around.... try to mate with them
                if (E.name.Equals(this.name) && E.age > 20)
                    other = true;
                
            if (other && amIAtHome() && EnoughFood())
            {
                chance = EnvironmentMap.Rand.NextDouble();
                if (chance < chanceToDuplicate)
                {
                    Agent a = new Agent(bnet, this.Home);
                    // children enherite there parents family
                    a.addFamily(Family);
                    // and the parents teach the child the lay of the land
                    a.teach(myMap);
                    EnvironmentMap.add(a, X, Y);
                }
            }

            return true;
        }

        private bool EnoughFood()
        {
            if (this.Home.name.Equals("Cave"))
            {
                Cave cave = (Cave)this.Home;
                if (cave.Inventory.ContainsKey("meat") 
                    && cave.Inventory.ContainsKey("fruit")
                    && (cave.Inventory["meat"] + cave.Inventory["fruit"]) > 50)
                    return true;
            }
            else if (this.Home.name.Equals("House"))
            {
                House house = (House)this.Home;
                if (house.Inventory.ContainsKey("meat")
                    && house.Inventory["meat"] > 50)
                    return true;
            }
            return false;
        }

        public override void Die()
        {
            // when an agent dies it "moves out" of its home
            if (Home != null && Home.name.Equals("Cave"))
            {
                Cave cave = (Cave)Home;
                cave.moveOut(this);
            }
            // well i'm dead... my family does not care for me anymore :P
            foreach(Agent a in Family)
                if(a != this)
                    a.Family.Remove(this);
            Family.Clear();

            base.Die();
        }
    }
}
