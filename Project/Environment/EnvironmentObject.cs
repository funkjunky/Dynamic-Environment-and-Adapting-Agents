using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BayesNetwork;

namespace Project.Classes
{
    public abstract class EnvironmentObject
    {
        public int X, Y;
        public int age = 1;
        public int priority = 0;
        public int maxSquare = 1;
        public bool iCanMove;
        protected int Speed;
        public string name;
        public string category = "";
        protected double chanceToDuplicate;
        protected Dictionary<string, bool> Destroys = new Dictionary<string,bool>();
        protected Dictionary<string, bool> DestroysCategory = new Dictionary<string,bool>();
        protected Dictionary<string, bool> cannotBeWith = new Dictionary<string,bool>();
        protected Dictionary<string, bool> cannotBeWithCategory = new Dictionary<string,bool>();
        public Dictionary<string, int> resouces = new Dictionary<string, int>();

        public virtual bool update()
        {
            this.updateAge();
            this.move();
            this.duplicate();
            return true;
        }

        public virtual void updateAge()
        {
            ++age;
        }

        public virtual bool isalive()
        {
            return true;
        }

        public virtual bool duplicate()
        {
            return true;
        }

        public virtual bool add()
        {
            /*
             * When you add an object to a grib position, it might need to destroy other objects
             * that are already in that position.
             * example:
             *  A new water cell would destroy a ground cell, tree cell, etc
             */

            HashSet<EnvironmentObject> O;
            if(!canMove(X,Y))
                return false;

            O = EnvironmentMap.getAll(X, Y);

            foreach (EnvironmentObject E in O)
                if ((Destroys.ContainsKey(E.name) && Destroys[E.name]) ||
                    (DestroysCategory.ContainsKey(E.category) && DestroysCategory[E.category]))
                {
                    E.Die();
                    EnvironmentMap.remove(E, X, Y);
                }
            return true;
        }

        public virtual bool move()
        {
            if (!this.iCanMove)
                return false;


            int count = 0;
            int wander = new int();
            int offsetX = X;
            int offsetY = Y;
            while (count < Speed)
            {
                wander = EnvironmentMap.Rand.Next(1, 100);
                /*
                 * Attempt to wonder around, this "creeps" have no plan or destination
                 * so they just wander around.
                 */
                if(wander <= 25)
                {
                    // try to go up
                    offsetY += 1;
                    if (offsetY >= EnvironmentMap.sizeY)
                        // failed attempt to move
                        offsetY -= 1;
                    else
                        if (!this.canMove(offsetX, offsetY))
                        {
                            //failed again
                            offsetY -= 1;
                            ++count;
                        }
                        else
                            ++count;
                    break;
                }
                else if(wander <= 50)
                {
                    // try to go to the right
                    offsetX += 1;
                    if (offsetX >= EnvironmentMap.sizeX)
                        // failed attempt to move
                        offsetX -= 1;
                    else
                        if (!this.canMove(offsetX, offsetY))
                        {
                            //failed again
                            offsetX -= 1;
                            ++count;
                        }
                        else
                            ++count;
                    break;
                }
                else if(wander <= 75)
                {
                    // try to go down
                    offsetY -= 1;
                    if (offsetY < 0)
                        // failed attempt to move
                        offsetY += 1;
                    else
                        if (!this.canMove(offsetX, offsetY))
                        {
                            //failed again
                            offsetY += 1;
                            ++count;
                        }
                        else
                            ++count;
                    break;
                }
                else
                {
                    // try to go to the left
                    offsetX -= 1;
                    if (offsetX < 0)
                        // failed attempt to move
                        offsetX += 1;
                    else
                        if (!this.canMove(offsetX, offsetY))
                        {
                            //failed again
                            offsetX += 1;
                            ++count;
                        }
                        else
                            ++count;
                }
            }
            EnvironmentMap.move(this, offsetX, offsetY);
            // finished wandering succesfully
            return true;
        }

        public virtual bool canMove(int X, int Y)
        {
            HashSet<EnvironmentObject> O;
            O = EnvironmentMap.getAll(X, Y);
            foreach (EnvironmentObject E in O)
                if (cannotBeWith.ContainsKey(E.name) && cannotBeWith[E.name])
                    return false;

            return true;
        }

        public virtual string toString()
        {
            return this.name;
        }

        public virtual void Die()
        {
        }
    }
}
