using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Project.Classes
{
    static class EnvironmentMap
    {
        public static int sizeX = 70;
        public static int sizeY = 50;
        public static ArrayList[,] envMap;
        public static Dictionary<EnvironmentObject, int[]> queueToBeRemoved;
        public static Dictionary<EnvironmentObject, int[]> queueToBeAdded;
        public static Random Rand;

        public static bool setup()
        {
            Rand = new Random();
            queueToBeRemoved = new Dictionary<EnvironmentObject, int[]>();
            queueToBeAdded = new Dictionary<EnvironmentObject, int[]>();
            envMap = new ArrayList[sizeX, sizeY];

            // Initialize all ArrayLists to default size (empty)
            for ( int row = 0; row < sizeX; ++row )
            {
                for ( int column = 0; column < sizeY; ++column )
                {
                    envMap[row,column] = new ArrayList();
                }
            }

            return true;
        }

        /* this does not work....
         * 
        public static int[] getPosition(EnvironmentObject E)
        {
            int[] position = new int[2];

            for (int column = 0; column < sizeY; ++column)
                for (int row = 0; row < sizeX; ++row)
                    if (envMap[row,column].Contains(E))
                    {
                        Console.WriteLine("");
                        position[0] = row;
                        position[1] = column;

                        Console.WriteLine(position[0] + " " + position[1]);
                        return position;
                    }
            return position;
        }
        */

        public static bool remove(EnvironmentObject E, int X, int Y)
        {
            //Console.WriteLine("Remove " + E.name + " at X : " + X + " Y : " + Y);
            int[] position = new int[2];
            position[0] = X;
            position[1] = Y;
            if(!queueToBeRemoved.ContainsKey(E))
                queueToBeRemoved.Add(E, position);
            else
                return false;
            return true;
        }

        private static void removeList()
        {
            int X;
            int Y;
            int index;

            foreach (KeyValuePair<EnvironmentObject, int[]> E in queueToBeRemoved)
            {
                
                X = E.Value[0];
                Y = E.Value[1];
                if (envMap[X, Y].Contains(E.Key))
                {
                    index = envMap[X, Y].IndexOf(E.Key);
                    // But I like return values RemoveAt!
                    envMap[X, Y].RemoveAt(index);

                }
            }
        }

        public static bool add(EnvironmentObject E, int X, int Y)
        {
            //Console.WriteLine("Added " + E.name + " at X : " + X + " Y : " + Y);
            int[] position = new int[2];
            position[0] = X;
            position[1] = Y;
            if(!queueToBeAdded.ContainsKey(E))
                queueToBeAdded.Add(E, position);
            return true;
        }

        private static void addList()
        {
            int X;
            int Y;

            foreach (KeyValuePair<EnvironmentObject, int[]> E in queueToBeAdded)
            {
                X = E.Value[0];
                Y = E.Value[1];

                E.Key.X = X;
                E.Key.Y = Y;
                try
                {
                    if (CountSquare(E.Key.name, X, Y) < E.Key.maxSquare
                       && E.Key.add())
                        envMap[X, Y].Add(E.Key);
                    //else
                    //Console.WriteLine("Failed to add " + E.Key.name + " at X : " + X + " Y : " + Y);
                }
                catch (Exception)
                { }
            }
        }

        private static void clearList()
        {
            queueToBeAdded.Clear();
            queueToBeRemoved.Clear();
        }

        // Find out if an EnvironmentObject that has the name "name" is near X, Y
        public static bool near(string name, int X, int Y, int count)
        {
            int num = 0;
            int direction = 1;
            while (direction < 4)
            {
                HashSet<EnvironmentObject> O;
                int offset = new int();
                switch (direction)
                {
                    /*
                     * Attempt to wonder around, this "creeps" have no plan or destination
                     * so they just wander around.
                     */
                    case 1:
                        // try to go up
                        offset = Y + 1;

                        if (offset >= EnvironmentMap.sizeY)
                            break;
                        else
                        {
                            O = EnvironmentMap.getAll(X, offset);
                            foreach (EnvironmentObject E in O)
                            {
                                if (E.name.Equals(name))
                                    ++num;
                                
                            }
                        }
                        break;
                    case 2:
                        offset = X + 1;
                        if (offset >= EnvironmentMap.sizeX)
                            break;
                        else
                        {
                            O = EnvironmentMap.getAll(offset, Y);
                            foreach (EnvironmentObject E in O)
                            {
                                if (E.name.Equals(name))
                                    ++num;
                            }
                        }
                        break;
                    case 3:
                        offset = Y - 1;
                        if (offset < 0)
                            break;
                        else
                        {
                            O = EnvironmentMap.getAll(X, offset);
                            foreach (EnvironmentObject E in O)
                            {
                                if (E.name.Equals(name))
                                    ++num;
                            }
                        }
                        break;
                        // try to go down
                    case 4:
                        // try to go to the left
                        offset = X - 1;
                        if (offset < 0)
                            break;
                        else
                        {
                            O = EnvironmentMap.getAll(offset, Y);
                            foreach (EnvironmentObject E in O)
                            {
                                if (E.name.Equals(name))
                                    ++num;
                            }
                        }
                        break;
                }
                ++direction;
            }
            if (count <= num)
                return true;
            else
                return false;
        }

        public static bool move(EnvironmentObject E, int X, int Y)
        {

            if (envMap[E.X,E.Y].Contains(E))
            {
                remove(E, E.X, E.Y);
                add(E, X, Y);
                E.X = X;
                E.Y = Y;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static HashSet<EnvironmentObject> getAll(int X, int Y)
        {
            HashSet<EnvironmentObject> T = new HashSet<EnvironmentObject>();
            try
            {
                foreach (EnvironmentObject E in envMap[X, Y])
                {
                    T.Add(E);
                }
            }
            catch(Exception)
            { }

            return T;
        }

        public static int CountSquare(string name, int X, int Y)
        {
            int num = 0;
            HashSet<EnvironmentObject> Objects = new HashSet<EnvironmentObject>();
            try
            {
                Objects = getAll(X, Y);
                foreach (EnvironmentObject E in Objects)
                {
                    if (E.name.Equals(name))
                        ++num;

                }
            }
            catch (Exception)
            { }

            return num;
        }

        public static void update(Dictionary<string, int> counter)
        {
            for (int column = 0; column < sizeY; ++column)
                for (int row = 0; row < sizeX; ++row)
                    foreach (EnvironmentObject E in envMap[row,column])
                    {
                        ++counter[E.name];
                        E.update();
                        E.isalive();
                    }
            /*
             * We can not remove/add an element from the map untill the above loops were done...
             * so i created this function
             */
            addList();
            removeList();
            clearList();
        }

        //TODO: Below is a dictionary of symbols:
        //
        //dolphin:  >
        //fish:     F
        //grass:    ,
        //ground:   .
        //jackal:   J
        //wolf:     W
        //monkey:   m
        //mountain: ^
        //rabbit:   R
        //shark:    s
        //tree:     T
        //water:    ~
        //weed:     w
        //Agents:   A
        public static void toString()
        {
            for (int column = 0; column < sizeY; ++column)
                for (int row = 0; row < sizeX; ++row)
                    outputSquare(row, column);
        }

        public static string outputSquare(int X, int Y)
        {
            EnvironmentObject O = (EnvironmentObject)envMap[X, Y][0];
            foreach (EnvironmentObject E in envMap[X, Y])
            {
                if (O.priority < E.priority)
                    O = E;
            }
            return O.toString();
        }

        public static String outputAgentSquare(int X, int Y)
        {
           
            try
            {
                EnvironmentObject O = (EnvironmentObject)envMap[X, Y][0];
                foreach (EnvironmentObject E in envMap[X, Y])
                {
                    if (O.priority < E.priority &&
                        (!(E.category.Equals("Animal") || E.category.Equals("Fish")) || E.name.Equals("Agent")))
                        O = E;
                }
                return O.toString();
            }
            catch (Exception)
            {
                //EnvironmentObject O = (EnvironmentObject)envMap[0, 0][0];
                //envMap[X, Y].Add();
                return "?";
            }
        }
    }
}
