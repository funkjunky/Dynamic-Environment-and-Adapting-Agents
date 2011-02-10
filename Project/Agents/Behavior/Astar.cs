using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Project.Classes;
using Project;

namespace Project.Classes
{
    public class Astar
    {
        public Astar()
        {
        }
        /* Ok lets explain how this works. First we will start by the Output.
         * 
         * OutPut : This function call returns a stack with the top elements being the next location you wish
         *          to go to and the bottom element is the destination. The stack is composed of int[2] which
         *          at location int[0] is the value of X and at location int[1] is the value of Y.
         * 
         */
        public Stack<int[]> findPath(EnvironmentObject O, int X, int Y)
        {
            Stack<int[]> Path = new Stack<int[]>();
            int CurrentX = O.X;
            int CurrentY = O.Y;
            /* NOTE :
             * 
             * The list Open and Closed are not optimal. If we run into problems with the search being to slow
             * we will need to change them from a list to a tree for faster look up. 
             * 
             */ 
            List<Node> Open = new List<Node>();
            List<Node> Closed = new List<Node>();
            // Default Huristic
            int MD = Math.Abs(CurrentX - X) + Math.Abs(CurrentY - Y);
            Node BestOption;
            Open.Add(new Node(CurrentX, CurrentY, MD, 0, null));
            do
            {
                if (Open.Count == 0) // its empty meaning we can't find a path :(
                    return Path; // We then return an empty path since there is no way to get to the destination
                BestOption = FindBestOption(Open);

                Closed.Add(BestOption);
                Open.Remove(BestOption);
                // lets check all the neighbors.
                // Can we move to them?
                // We need to check all 4 neighbors.... :(
                if (checkNode(BestOption.X + 1, BestOption.Y, Open, Closed, BestOption))
                    if (O.canMove(BestOption.X + 1, BestOption.Y))
                    {
                        MD = Math.Abs(BestOption.X + 1 - X) + Math.Abs(BestOption.Y - Y);
                        Open.Add(new Node(BestOption.X + 1, BestOption.Y, MD, BestOption.G + 1, BestOption));
                    }
                if (checkNode(BestOption.X - 1, BestOption.Y, Open, Closed, BestOption))
                    if (O.canMove(BestOption.X - 1, BestOption.Y))
                    {
                        MD = Math.Abs(BestOption.X - 1 - X) + Math.Abs(BestOption.Y - Y);
                        Open.Add(new Node(BestOption.X - 1, BestOption.Y, MD, BestOption.G + 1, BestOption));
                    }
                if (checkNode(BestOption.X, BestOption.Y + 1, Open, Closed, BestOption))
                    if (O.canMove(BestOption.X, BestOption.Y + 1))
                    {
                        MD = Math.Abs(BestOption.X - X) + Math.Abs(BestOption.Y + 1 - Y);
                        Open.Add(new Node(BestOption.X, BestOption.Y + 1, MD, BestOption.G + 1, BestOption));
                    }
                if (checkNode(BestOption.X, BestOption.Y - 1, Open, Closed, BestOption))
                    if (O.canMove(BestOption.X, BestOption.Y - 1))
                    {
                        MD = Math.Abs(BestOption.X - X) + Math.Abs(BestOption.Y - 1 - Y);
                        Open.Add(new Node(BestOption.X, BestOption.Y - 1, MD, BestOption.G + 1, BestOption));
                    }
                // set the current X and Y to the Best Option previously defined in this iterastion of the loop
                CurrentX = BestOption.X;
                CurrentY = BestOption.Y;
            }
            while (CurrentX != X ||
                  CurrentY != Y);
            // Now we need to generate the path
            Path = GeneratePath(BestOption);

            return Path;
        }

        private Node FindBestOption(List<Node> Open)
        {
            // Searchs the current list of Open nodes for the one with the best F
            Node Best = Open.Last<Node>();
            foreach(Node N in Open)
                if (Best.TotalDistance() > N.TotalDistance())
                    Best = N;
            return Best;
        }

        private Stack<int[]> GeneratePath(Node N)
        {
            Stack<int[]> Path = new Stack<int[]>();
            

            // We go trough all the parents until we reach one that is null ( the starting point ) and put them
            // all into a stack.
            while (N.Parent != null)
            {
                Path.Push(new int[]{N.X, N.Y});
                N = N.Parent;
            }
            // Right now the stack is upsidedown so we need to reverse the elements in the stack... this is how
            Stack<int[]> FinalPath = new Stack<int[]>(Path);

            return FinalPath;

        }

        private bool checkNode(int X, int Y, List<Node> Open, List<Node> Closed, Node Parent)
        {
            // check if node exists first
            if (X < 0 || X >= EnvironmentMap.sizeX ||
                Y < 0 || Y >= EnvironmentMap.sizeY)
                return false;
            // Check is nodes are in the Closed or Open list.
            if(isClosed(X, Y, Closed, Parent))
                return false;
            if (isOpen(X, Y, Open, Parent))
                return false;

            return true;
        }

        private bool isClosed(int X, int Y, List<Node> Closed, Node Parent)
        {
            foreach (Node N in Closed)
                if (N.X == X && N.Y == Y)
                {
                    // If the node is found in the closed list update it if the G is bigger then the current G
                    // This means we found a better path to get to this point.
                    if (N.G > Parent.G + 1)
                    {
                        N.G = Parent.G + 1;
                        N.Parent = Parent;
                    }
                    return true;
                }

            return false;
        }

        private bool isOpen(int X, int Y, List<Node> Open, Node Parent)
        {
            foreach (Node N in Open)
                if (N.X == X && N.Y == Y)
                {
                    // If the node is found in the open list update it if the G is bigger then the current G
                    // This means we found a better path to get to this point.
                    if (N.G > Parent.G + 1)
                    {
                        N.G = Parent.G + 1;
                        N.Parent = Parent;
                    }
                    return true;
                }
            return false;
        }
    }

    class Node
    {
        // Node class to keep relavent information about each of the nodes....
        public int X;
        public int Y;
        public int MD;
        public int G;
        public Node Parent;

        public Node(int x, int y, int md, int g, Node parent)
        {
            X = x;
            Y = y;
            MD = md;
            G = g;
            Parent = parent;
        }

        public int TotalDistance()
        {
            return MD + G;
        }
    }
}
